using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public enum JOYCON_BUTTON_LEFT
    {
        DPAD_DOWN = 0,
        DPAD_RIGHT = 1,
        DPAD_LEFT = 2,
        DPAD_UP = 3,
        SL = 4,
        SR = 5,
        MINUS = 6,
        HOME = 7,
        PLUS = 8,
        CAPTURE = 9,
        STICK = 10,
        L = 11,
        ZL = 12,
    }

    public enum JOYCON_BUTTON_RIGHT
    {
        DPAD_B = 0,
        DPAD_A = 1,
        DPAD_Y = 2,
        DPAD_X = 3,
        SL = 4,
        SR = 5,
        MINUS = 6,
        HOME = 7,
        PLUS = 8,
        CAPTURE = 9,
        STICK = 10,
        R = 11,
        ZR = 12,
    }

    /*public enum JOYCON_ACTION
    {
        PUNCH = 0,          // パンチ
        SPECIAL_SKILL = 1,  // 必殺技
        TWIST = 2,          // ひねったか
        SHAKE = 3           // 振ったか
    }

    public enum JOYCON_MOTION
    {
        ACCLEL = 0, // 加速度
        GYRO = 1,   // 回転速度
        SLOPE = 2   // 傾き
    }*/



    private static readonly Joycon.Button[] m_buttons =
        Enum.GetValues(typeof(Joycon.Button)) as Joycon.Button[];

    private List<Joycon> m_joycons;
    private Joycon m_joyconL;
    private Joycon m_joyconR;
    private Joycon.Button? m_pressedButtonL;
    private Joycon.Button? m_pressedButtonR;

    private const int JoyconType = 2;        // ジョイコン数
    private const int FrameWaitArray = 200;  // 加速度を保持するフレーム数
    private Vector3[,] m_AccelBuff = new Vector3[JoyconType, FrameWaitArray];   // 加速度を保持
    private Vector3[,] m_GyroBuff = new Vector3[JoyconType, FrameWaitArray];    // 回転速度を保持
    private bool[] m_IsPunch = new bool[JoyconType];    // パンチしたか
    private bool m_IsSpecialSkill = false;  // 必殺技をしたか
    private int m_PushLRWait = 0;           // 必殺技時、LRを離しても少しの間だったら出るようにするためのフレーム
    private int m_JoyconRDelay = 5;         // 2つ目ジョイコンのずれフレーム
    private bool[] m_IsVibrationPunchiHit = new bool[JoyconType];   // パンチヒットの振動をならしているか
    private bool m_IsVibrationDamage = false;   // ダメージ受けた時の振動をならしているか

    private bool IsDebug = true;
    public GameObject m_DbgTextUI;
    public int m_DbgNumPunchi;

    
    private void Start()
    {
        m_joycons = JoyconManager.Instance.j;

        if (m_joycons == null || m_joycons.Count <= 0) return;

        m_joyconL = m_joycons.Find(c => c.isLeft);
        m_joyconR = m_joycons.Find(c => !c.isLeft);


        for (int i = 0; i < FrameWaitArray; i++)
        {
            m_AccelBuff[0, i] = new Vector3(0.0f, 0.0f, 0.0f);
            m_AccelBuff[1, i] = new Vector3(0.0f, 0.0f, 0.0f);
        }

        m_IsPunch[0] = false;
        m_IsPunch[1] = false;

        m_IsSpecialSkill = false;

        m_IsVibrationPunchiHit[0] = false;
        m_IsVibrationPunchiHit[1] = false;

        m_DbgNumPunchi = 0;

    }

    private void Update()
    {
        Storage();

        Punch();

        SpecialSkill();
        
        VibrationUpdate();

        SyncJoycon();

        //if (IsDebug) Dbg();
    }


    // 左ジョイコンのボタンの取得
    public bool GetPressButtonL(JOYCON_BUTTON_LEFT button)
    {
        return m_joyconL.GetButton((Joycon.Button)button);
    }
    public bool GetTriggerButtonL(JOYCON_BUTTON_LEFT button)
    {
        return m_joyconL.GetButtonDown((Joycon.Button)button);
    }
    public bool GetReleaseButtonL(JOYCON_BUTTON_LEFT button)
    {
        return m_joyconL.GetButtonUp((Joycon.Button)button);
    }
    
    // 右ジョイコンのボタンの取得
    public bool GetPressButtonR(JOYCON_BUTTON_RIGHT button)
    {
        return m_joyconR.GetButton((Joycon.Button)button);
    }
    public bool GetTriggerButtonR(JOYCON_BUTTON_RIGHT button)
    {
        return m_joyconR.GetButtonDown((Joycon.Button)button);
    }
    public bool GetReleaseButtonR(JOYCON_BUTTON_RIGHT button)
    {
        return m_joyconR.GetButtonUp((Joycon.Button)button);
    }

    // パンチしたか取得
    public bool GetPunchL()
    {
        return m_IsPunch[0];
    }
    public bool GetPunchR()
    {
        return m_IsPunch[1];
    }

    // 必殺技をしたか取得
    public bool GetSpecialSkill()
    {
        return m_IsSpecialSkill;
    }

    // 加速度を取得
    public Vector3 GetAccelL()
    {
        return m_joyconL.GetAccel();
    }
    public Vector3 GetAccelR()
    {
        return m_joyconR.GetAccel();
    }

    // 回転速度を取得
    public Vector3 GetGyroL()
    {
        return m_joyconL.GetGyro();
    }
    public Vector3 GetGyroR()
    {
        return m_joyconR.GetGyro();
    }

    // 傾きを取得
    public Quaternion GetSlopeL()
    {
        return m_joyconL.GetVector();
    }
    public Quaternion GetSlopeR()
    {
        return m_joyconR.GetVector();
    }

    // ひねりの回転の大きさを取得
    public float GetTwistL()
    {
        return m_joyconL.GetGyro().magnitude;
    }
    public float GetTwistR()
    {
        return m_joyconR.GetGyro().magnitude;
    }

    // 振った大きさを取得
    public float GetShakeL()
    {
        return m_joyconL.GetAccel().magnitude;
    }
    public float GetShakeR()
    {
        return m_joyconR.GetAccel().magnitude;
    }

    // 振動させる
    public void SetVibrationL(float width, float height, float power, int time)
    {
        m_joyconL.SetRumble(width, height, power, time);
    }
    public void SetVibrationR(float width, float height, float power, int time)
    {
        m_joyconR.SetRumble(width, height, power, time);
    }
    public void SetVibration(bool isLeft, float width, float height, float power, int time)
    {
        if(isLeft == true) m_joyconL.SetRumble(width, height, power, time);
        else m_joyconR.SetRumble(width, height, power, time);
    }

    // パンチの振動をする
    public void VibrationPunchiShotL()
    {
        SetVibrationL(300.0f, 300.0f, 0.7f, 60);
    }
    public void VibrationPunchiShotR()
    {
        SetVibrationR(300.0f, 300.0f, 0.7f, 60);
    }

    // パンチが当たった時の振動をする
    public void VibrationPunchiHitL()
    {
        m_IsVibrationPunchiHit[0] = true;
    }
    public void VibrationPunchiHitR()
    {
        m_IsVibrationPunchiHit[1] = true;
    }

    // ダメージを受けた時の振動をする
    public void VibrationDamage()
    {
        m_IsVibrationDamage = true;
    }

    // 後でクラス化したい
    private void Punch()
    {
        m_IsPunch[0] = false;
        m_IsPunch[1] = false;

        if (GetPressButtonL(JOYCON_BUTTON_LEFT.L) == true || GetPressButtonR(JOYCON_BUTTON_RIGHT.R) == true)
            return;

        for (int joyconType = 0; joyconType < 2; joyconType++)
        {
            // ジャイロが大きければ処理しない
            int GyroCount = 0;
            for (int i = 0; i < 7; i++)
            {
                if(m_GyroBuff[joyconType, i].magnitude > 12.0f)
                {
                    //if(m_GyroBuff[joyconType, i].z < 19.0f)
                    //{
                        //Debug.Log("ジャイロが大きい：" + i + "   回転速度：" + m_GyroBuff[joyconType, i].magnitude + "Z回転：" + m_GyroBuff[joyconType, i].z);
                        GyroCount++;
                    //}
                    //else
                    //{
                      //  Debug.Log("例外：" + i + "   回転速度：" + m_GyroBuff[joyconType, i].magnitude + "Z回転：" + m_GyroBuff[joyconType, i].z);
                    //}
                }
            }
            if(GyroCount >= 3)
            {
                Debug.Log("ジャイロオーバー！");
                continue;
            }

            

            float maxSpeed = -100.0f;   // 最大速度
            int AccelPlusCount = 0;     // 前に加速したフレーム数
            for (int i = 0; i < 10; i++)
            {
                if (m_AccelBuff[joyconType, i].y > 0.3f)
                {
                    AccelPlusCount++;
                    //Debug.Log("成功:" + AccelPlusCount + " 速さ:" + m_AccelBuff[joyconType, i].y);
                }
                if (maxSpeed < m_AccelBuff[joyconType, i].y) maxSpeed = m_AccelBuff[joyconType, i].y;
            }

            bool IsAccelPlus = false;   // 突き出す操作の成功可否
            if (AccelPlusCount >= 5)
            {
                IsAccelPlus = true;
            }

            if (maxSpeed < 0.0f) continue;

            // 突き出しつつ、最後に止めたときにパンチ
            if (maxSpeed - m_AccelBuff[joyconType, 0].y > 2.0f && m_AccelBuff[joyconType, 0].y < -0.8f && IsAccelPlus == true)
            {
                m_IsPunch[joyconType] = true;
                m_DbgNumPunchi++;

                float defference = maxSpeed - m_AccelBuff[joyconType, 0].y;
                Debug.Log("パンチした！" + " 速さ:" + m_AccelBuff[joyconType, 0].y + " 最大の速さ:" + maxSpeed + " その差" + defference);
                //Debug.Log("→回転速度：" + m_GyroBuff[joyconType, 0].magnitude + "Z回転：" + m_GyroBuff[joyconType, 0].z);
            }
        }
    }

    // 必殺技の処理
    private void SpecialSkill()
    {
        m_IsSpecialSkill = false;

        //if (GetPressButtonL(JOYCON_BUTTON_LEFT.L) == false || GetPressButtonR(JOYCON_BUTTON_RIGHT.R) == false)
          //  return;

        if (GetPressButtonL(JOYCON_BUTTON_LEFT.L) == true && GetPressButtonR(JOYCON_BUTTON_RIGHT.R) == true)
        {
            m_PushLRWait = 15;
        }

        m_PushLRWait--;
        if (m_PushLRWait < 0) m_PushLRWait = 0;

        if (m_PushLRWait <= 0) return;

        
        int AccelCount = 0;
        int GyroCount = 0;


        for (int joyconType = 0; joyconType < 2; joyconType++)
        {
            float minSpeed = 100.0f;   // 最小速度
            int AccelPlusCount = 0;
            for (int i = 0 + (1 - joyconType) * m_JoyconRDelay; i < 20 + (1 - joyconType) * m_JoyconRDelay; i++)
            {
                if (m_AccelBuff[joyconType, i].x < 0.2f)
                {
                    AccelPlusCount++;
                }
                if (minSpeed > m_AccelBuff[joyconType, i].x) minSpeed = m_AccelBuff[joyconType, i].x;
            }

            bool IsAccelPlus = false;   // 振り下ろす操作の成功可否
            if (AccelPlusCount >= 8)
            {
                IsAccelPlus = true;
            }

            // 振り下ろしつつ、最後に止めたときに必殺技
            if (minSpeed - m_AccelBuff[joyconType, (1 - joyconType) * m_JoyconRDelay].x < -1.0 && m_AccelBuff[joyconType, (1 - joyconType) * m_JoyconRDelay].x > 0.2f && IsAccelPlus == true)
            {
                AccelCount++;
                //Debug.Log("必殺技加速成功カウントプラス");
            }

            for (int i = 0 + (1 - joyconType) * m_JoyconRDelay; i < 3 + (1 - joyconType) * m_JoyconRDelay; i++)
            {
                if (m_GyroBuff[joyconType, (1 - joyconType) * m_JoyconRDelay].z > 4.0f)
                {
                    GyroCount++;
                    //Debug.Log("必殺技回転成功カウントプラス");
                    break;
                }
            }
        }
        if (AccelCount >= 2)
        {
            //Debug.Log("アクセル成功！" + m_AccelBuff[0, m_JoyconRDelay] + "  " + m_AccelBuff[1, 0]);
            if (GyroCount >= 2)
            {
                m_IsSpecialSkill = true;
                Debug.Log("必殺技！");
                Debug.Log(m_GyroBuff[0, m_JoyconRDelay] + "  " + m_GyroBuff[1, 0]);
            }
            else
            {
                //Debug.Log("ジャイロカウントが足りない！" + m_GyroBuff[0, m_JoyconRDelay] + "  " + m_GyroBuff[1, 0]);
            }
            

        }


    }

    // フレーム保管の更新
    private void Storage()
    {
        for (int joyconType = 0; joyconType < 2; joyconType++)
        {
            for (int i = FrameWaitArray - 1; i >= 1; i--)
            {
                m_AccelBuff[joyconType, i] = m_AccelBuff[joyconType, i - 1];
                m_GyroBuff[joyconType, i] = m_GyroBuff[joyconType, i - 1];
            }
            if (joyconType == 0)
            {
                m_AccelBuff[joyconType, 0] = m_joyconL.GetAccel();
                m_GyroBuff[joyconType, 0] = m_joyconL.GetGyro();
            }
            else
            {
                m_AccelBuff[joyconType, 0] = m_joyconR.GetAccel();
                m_AccelBuff[joyconType, 0].y *= -1.0f;
                m_AccelBuff[joyconType, 0].z *= -1.0f;

                m_GyroBuff[joyconType, 0] = m_joyconR.GetGyro();
                m_GyroBuff[joyconType, 0].y *= -1.0f;
                m_GyroBuff[joyconType, 0].z *= -1.0f;
            }
        }

        //Debug.Log("加速度Left：" + m_AccelBuff[0, 0] + "   回転速度Left：" + m_GyroBuff[0, 0]);
        //Debug.Log("傾き：" + GetSlopeL().eulerAngles / 360.0f);
    }

    private int delayFrame = 0;
    // ジョイコンの同期を取る
    private void SyncJoycon()
    {
        if (GetTriggerButtonL(JOYCON_BUTTON_LEFT.DPAD_RIGHT))
        {
            delayFrame = 0;
        }

            if (GetPressButtonL(JOYCON_BUTTON_LEFT.ZL) && GetPressButtonR(JOYCON_BUTTON_RIGHT.ZR))
        {
            if (GetPressButtonL(JOYCON_BUTTON_LEFT.DPAD_RIGHT))
            {
                if (GetPressButtonR(JOYCON_BUTTON_RIGHT.DPAD_A))
                {
                    m_JoyconRDelay = delayFrame;
                    Debug.Log("右ジョイコンの遅延：" + delayFrame);
                }
                else
                {
                    delayFrame++;
                }
            }
            else
            {
                
            }
        }
    }

    // 振動処理の更新
    private void VibrationUpdate()
    {
        VibrationPunchiHitUpdate();
        VibrationDamageUpdate();
    }
    

    private int[] VibCountPunch = new int[JoyconType];
    // パンチヒット時の振動処理
    private void VibrationPunchiHitUpdate()
    {
        for(int i = 0; i < JoyconType; i++)
        {
            if (m_IsVibrationPunchiHit[i] == false) continue;

            bool isLeft = false;
            if (i == 0) isLeft = true;
            else isLeft = false;

            if (VibCountPunch[i] < 3)
            {
                SetVibration(isLeft, 180.0f, 180.0f, 0.1f, 1);
            }
            else if (VibCountPunch[i] < 5)
            {
                SetVibration(isLeft, 180.0f, 180.0f, 0.4f, 1);
            }
            else if (VibCountPunch[i] < 12)
            {
                SetVibration(isLeft, 180.0f, 180.0f, 0.2f, 1);
            }
            else if (VibCountPunch[i] < 15)
            {
                SetVibration(isLeft, 180.0f, 180.0f, 0.1f, 1);
            }
            else
            {
                VibCountPunch[i] = 0;
                m_IsVibrationPunchiHit[i] = false;
            }

            VibCountPunch[i]++;
        }

        
    }

    private int VibCountDamage = 0;
    // ダメージを受けた時の振動処理
    private void VibrationDamageUpdate()
    {
        if (m_IsVibrationDamage == false) return;


        if (VibCountDamage < 3)
        {
            SetVibrationL(180.0f, 180.0f, 0.2f, 1);
            SetVibrationR(180.0f, 180.0f, 0.2f, 1);
        }
        else if (VibCountDamage < 5)
        {
            SetVibrationL(180.0f, 180.0f, 0.6f, 1);
            SetVibrationR(180.0f, 180.0f, 0.6f, 1);
        }
        else if (VibCountDamage < 8)
        {
            SetVibrationL(180.0f, 180.0f, 0.3f, 1);
            SetVibrationR(180.0f, 180.0f, 0.3f, 1);
        }
        else if (VibCountDamage < 10)
        {
            SetVibrationL(180.0f, 180.0f, 0.5f, 1);
            SetVibrationR(180.0f, 180.0f, 0.5f, 1);
        }
        else if (VibCountDamage < 18)
        {
            SetVibrationL(180.0f, 180.0f, 0.2f, 1);
            SetVibrationR(180.0f, 180.0f, 0.2f, 1);
        }
        else if (VibCountDamage < 24)
        {
            SetVibrationL(180.0f, 180.0f, 0.1f, 1);
            SetVibrationR(180.0f, 180.0f, 0.1f, 1);
        }
        else
        {
            VibCountDamage = 0;
            m_IsVibrationDamage = false;
        }

        VibCountDamage++;
    }


    private float dbgVib = 0.0f;
    private void Dbg()
    {
        Text DbgTextUI = m_DbgTextUI.GetComponent<Text>();

        DbgTextUI.text = m_DbgNumPunchi.ToString();


        if (GetTriggerButtonR(JOYCON_BUTTON_RIGHT.DPAD_B))
        {
            VibrationDamage();
        }
        if (GetTriggerButtonR(JOYCON_BUTTON_RIGHT.DPAD_Y))
        {
            VibrationPunchiHitR();
        }
        if (GetTriggerButtonR(JOYCON_BUTTON_RIGHT.DPAD_X))
        {
            VibrationPunchiShotR();
        }


        if (GetTriggerButtonL(JOYCON_BUTTON_LEFT.DPAD_RIGHT))
        {
            VibrationPunchiHitL();
        }
        if (GetTriggerButtonR(JOYCON_BUTTON_RIGHT.DPAD_A))
        {
            VibrationPunchiHitR();
        }
        if (GetTriggerButtonL(JOYCON_BUTTON_LEFT.DPAD_UP))
        {
            VibrationDamage();
        }
    }
}

