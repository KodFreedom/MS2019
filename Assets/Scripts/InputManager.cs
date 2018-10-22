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

    public enum JOYCON_KEYBOARD
    {
        PUNCH_L = KeyCode.A,        // 左パンチのキー
        PUNCH_R = KeyCode.D,        // 右パンチのキー
        SPECIAL_SKILL = KeyCode.S,  // 必殺技のキー
        MODE_CHANGE = KeyCode.W     // 放電モード切替のキー
    }

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
    //private int m_PushLRWait = 0;           // 必殺技時、LRを離しても少しの間だったら出るようにするためのフレーム
    private int m_JoyconRDelay = 5;         // 2つ目ジョイコンのずれフレーム
    private bool[] m_IsVibrationPunchiHit = new bool[JoyconType];   // パンチヒットの振動をならしているか
    private bool[] m_IsVibrationPunchiShot = new bool[JoyconType];   // パンチした時の振動をならしているか
    private bool m_IsVibrationDamage = false;   // ダメージ受けた時の振動をならしているか
	private bool m_IsVibrationSkill = false;   // ダメージ受けた時の振動をならしているか
    private bool m_KeybordMode = false;        // キーボードでパンチや必殺技を打てるようにするか
    private bool m_IsThunderMode = false;
    private int m_VibDamageLoopCount = 3;
    private int m_VibSkillTime = 20;
    private int m_VibPunchShotTime = 5;

    private bool IsDebug = true;
    public GameObject m_DbgTextUI;
    public int m_DbgNumPunchi;

    
    private void Start()
    {
        m_joycons = JoyconManager.Instance.j;

        m_joyconL = null;
        m_joyconR = null;
        if (m_joycons != null && m_joycons.Count > 0)
        {
            m_joyconL = m_joycons.Find(c => c.isLeft);
            m_joyconR = m_joycons.Find(c => !c.isLeft);
        }
        

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

        ThunderModeUpdate();

        VibrationUpdate();

        SyncJoycon();

        KeyboardDebug();

        if (IsDebug) Dbg();
    }


    // 左ジョイコンのボタンの取得
    public bool GetPressButtonL(JOYCON_BUTTON_LEFT button)
    {
	    if(m_joycons == null || m_joyconL == null)
	    	return false;
        return m_joyconL.GetButton((Joycon.Button)button);
    }
    public bool GetTriggerButtonL(JOYCON_BUTTON_LEFT button)
    {
	    if(m_joycons == null || m_joyconL == null)
	    	return false;
        return m_joyconL.GetButtonDown((Joycon.Button)button);
    }
    public bool GetReleaseButtonL(JOYCON_BUTTON_LEFT button)
    {
	    if(m_joycons == null || m_joyconL == null)
	    	return false;
        return m_joyconL.GetButtonUp((Joycon.Button)button);
    }
    
    // 右ジョイコンのボタンの取得
    public bool GetPressButtonR(JOYCON_BUTTON_RIGHT button)
    {
	    if(m_joycons == null || m_joyconR == null)
	    	return false;
        return m_joyconR.GetButton((Joycon.Button)button);
    }
    public bool GetTriggerButtonR(JOYCON_BUTTON_RIGHT button)
    {
	    if(m_joycons == null || m_joyconR == null)
	    	return false;
        return m_joyconR.GetButtonDown((Joycon.Button)button);
    }
    public bool GetReleaseButtonR(JOYCON_BUTTON_RIGHT button)
    {
	    if(m_joycons == null || m_joyconR == null)
	    	return false;
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

    // 放電モードかどうか取得
    public bool GetThunderMode()
    {
        return m_IsThunderMode;
    }

    // 放電モードの状態を変更
    public void SetThunderMode(bool mode)
    {
        m_IsThunderMode = mode;
    }

    // 加速度を取得
    public Vector3 GetAccelL()
    {
	    if(m_joycons == null || m_joyconL == null)
	    	return new Vector3(0.0f, 0.0f, 0.0f);
        return m_joyconL.GetAccel();
    }
    public Vector3 GetAccelR()
    {
	    if(m_joycons == null || m_joyconR == null)
	    	return new Vector3(0.0f, 0.0f, 0.0f);
        return m_joyconR.GetAccel();
    }

    // 回転速度を取得
    public Vector3 GetGyroL()
    {
	    if(m_joycons == null || m_joyconL == null)
	    	return new Vector3(0.0f, 0.0f, 0.0f);
        return m_joyconL.GetGyro();
    }
    public Vector3 GetGyroR()
    {
	    if(m_joycons == null || m_joyconR == null)
	    	return new Vector3(0.0f, 0.0f, 0.0f);
        return m_joyconR.GetGyro();
    }

    // 傾きを取得（ライブラリの関係でうまく取れない）
    public Quaternion GetSlopeL()
    {
    	if(m_joycons == null || m_joyconL == null)
	    	return Quaternion.identity;
        return m_joyconL.GetVector();
    }
    public Quaternion GetSlopeR()
    {
    	if(m_joycons == null || m_joyconR == null)
		    return Quaternion.identity;
        return m_joyconR.GetVector();
    }

    // ひねりの回転の大きさを取得
    public float GetTwistL()
    {
	    if(m_joycons == null || m_joyconL == null)
		    return 0.0f;
        return m_joyconL.GetGyro().magnitude;
    }
    public float GetTwistR()
    {
	    if(m_joycons == null || m_joyconR == null)
		    return 0.0f;
        return m_joyconR.GetGyro().magnitude;
    }

    // 振った大きさを取得
    public float GetShakeL()
    {
	    if(m_joycons == null || m_joyconL == null)
	    	return 0.0f;
        return m_joyconL.GetAccel().magnitude;
    }
    public float GetShakeR()
    {
	    if(m_joycons == null || m_joyconR == null)
	    	return 0.0f;
        return m_joyconR.GetAccel().magnitude;
    }

    // 振動させる
    public void SetVibrationL(float width, float height, float power, int time)
    {
    	if(m_joycons == null || m_joyconL == null)
		    return;
        m_joyconL.SetRumble(width, height, power, time);
    }
    public void SetVibrationR(float width, float height, float power, int time)
    {
	    if(m_joycons == null || m_joyconR == null)
	    	return;
        m_joyconR.SetRumble(width, height, power, time);
    }
    public void SetVibration(bool isLeft, float width, float height, float power, int time)
    {
	    if(m_joycons == null || m_joyconL == null || m_joyconL == null)
	    	return;
        if(isLeft == true) m_joyconL.SetRumble(width, height, power, time);
        else m_joyconR.SetRumble(width, height, power, time);
    }

    // パンチの振動をする
    public void VibrationPunchiShotL(int time = 5)
    {
	    if(m_joycons == null || m_joyconL == null)
	    	return;

        m_VibPunchShotTime = time;
        m_IsVibrationPunchiShot[0] = true;
    }
    public void VibrationPunchiShotR(int time = 5)
    {
	    if(m_joycons == null || m_joyconR == null)
	    	return;
        m_VibPunchShotTime = time;
        m_IsVibrationPunchiShot[0] = true;
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
    public void VibrationDamage(int loopCount = 3)
    {
        m_IsVibrationDamage = true;
        m_VibDamageLoopCount = loopCount;
    }

    // 必殺技時の振動をする
    public void VibrationSkill(int time = 20)
    {
        m_IsVibrationSkill = true;
        m_VibSkillTime = time;
    }

    // 後でクラス化したい
    private void Punch()
    {
        m_IsPunch[0] = false;
        m_IsPunch[1] = false;

        //if (GetPressButtonL(JOYCON_BUTTON_LEFT.L) == true || GetPressButtonR(JOYCON_BUTTON_RIGHT.R) == true)
        //    return;

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
                //Debug.Log("ジャイロオーバー！");
                continue;
            }

            int AccelXCount = 0;
            for (int i = 0; i < 12; i++)
            {
                if (m_AccelBuff[joyconType, i].x < -0.4)
                {
                    AccelXCount++;
                }
            }
            if (AccelXCount >= 4)
            {
                //Debug.Log("必殺技と区別");
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
        
        
        int AccelCount = 0;
        int GyroCount = 0;


        for (int joyconType = 0; joyconType < 2; joyconType++)
        {

            // ひねっていたら除外
            int twistCount = 0;
            for (int i = 0 + (1 - joyconType) * m_JoyconRDelay; i < 10 + (1 - joyconType) * m_JoyconRDelay; i++)
            {
                if (m_GyroBuff[joyconType, (1 - joyconType) * m_JoyconRDelay].magnitude > 19.0f)
                {
                    twistCount++;
                }
            }
            if (twistCount >= 6)
            {
                //Debug.Log("ひねっている(必殺技)");
                continue;
            }




            float minSpeed = 100.0f;   // 最小速度
            int AccelPlusCount = 0;
            for (int i = 0 + (1 - joyconType) * m_JoyconRDelay; i < 20 + (1 - joyconType) * m_JoyconRDelay; i++)
            {
                if (m_AccelBuff[joyconType, i].x < 0.4f)
                {
                    AccelPlusCount++;
                }
                if (minSpeed > m_AccelBuff[joyconType, i].x) minSpeed = m_AccelBuff[joyconType, i].x;
            }

            bool IsAccelPlus = false;   // 振り下ろす操作の成功可否
            if (AccelPlusCount >= 12)
            {
                IsAccelPlus = true;
            }

            // 振り下ろしつつ、最後に止めたときに必殺技
            if (minSpeed - m_AccelBuff[joyconType, (1 - joyconType) * m_JoyconRDelay].x < -1.4 && m_AccelBuff[joyconType, (1 - joyconType) * m_JoyconRDelay].x > 0.2f && IsAccelPlus == true)
            {
                //float defference = minSpeed - m_AccelBuff[joyconType, (1 - joyconType) * m_JoyconRDelay].x;
                //Debug.Log(" アクセル成功！" + " 速さ:" + m_AccelBuff[joyconType, 0].x + " 最大の速さ:" + minSpeed + " その差" + defference);
                AccelCount++;
                //Debug.Log("必殺技加速成功カウントプラス");
            }

            for (int i = 0 + (1 - joyconType) * m_JoyconRDelay; i < 3 + (1 - joyconType) * m_JoyconRDelay; i++)
            {
                if (m_GyroBuff[joyconType, (1 - joyconType) * m_JoyconRDelay].z > 3.0f)
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
                //Debug.Log(m_GyroBuff[0, m_JoyconRDelay] + "  " + m_GyroBuff[1, 0]);
                
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
                m_AccelBuff[joyconType, 0] = GetAccelL();
                m_GyroBuff[joyconType, 0] = GetGyroL();
            }
            else
            {
                m_AccelBuff[joyconType, 0] = GetAccelR();
                m_AccelBuff[joyconType, 0].y *= -1.0f;
                m_AccelBuff[joyconType, 0].z *= -1.0f;

                m_GyroBuff[joyconType, 0] = GetGyroR();
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
        }
    }

    // 振動処理の更新
    private void VibrationUpdate()
    {
        VibrationPunchiHitUpdate();
        VibrationDamageUpdate();
        VibrationSkillUpdate();
        VibrationPunchiShotUpdate();
    }

    private int[] VibCountPunchShot = new int[JoyconType];
    // パンチヒット時の振動処理
    private void VibrationPunchiShotUpdate()
    {
        for (int i = 0; i < JoyconType; i++)
        {
            if (m_IsVibrationPunchiShot[i] == false) continue;

            bool isLeft = false;
            if (i == 0) isLeft = true;
            else isLeft = false;

            if (VibCountPunchShot[i] < m_VibPunchShotTime) // フェードアウト
            {
                SetVibration(isLeft, 300.0f, 300.0f, 0.7f, 1);
            }
            else
            {
                VibCountPunchShot[i] = 0;
                m_IsVibrationPunchiShot[i] = false;
            }

            VibCountPunchShot[i]++;
        }
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

            if (VibCountPunch[i] < 2)   // フェードイン
            {
                SetVibration(isLeft, 180.0f, 180.0f, 0.2f, 1);
            }
            else if (VibCountPunch[i] < 6)  // 拳に来る振動
            {
                SetVibration(isLeft, 180.0f, 180.0f, 0.45f, 1);
            }
            else if (VibCountPunch[i] < 10) // 振動引き
            {
                SetVibration(isLeft, 180.0f, 180.0f, 0.2f, 1);
            }
            else if (VibCountPunch[i] < 12) // フェードアウト
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


        if (VibCountDamage < 2)     // フェードイン
        {
            SetVibrationL(180.0f, 180.0f, 0.2f, 1);
            SetVibrationR(180.0f, 180.0f, 0.2f, 1);
        }
        else if (VibCountDamage < 4 + (m_VibDamageLoopCount - 1) * 4 && (VibCountDamage - 2) % 4 < 2 )    // 強い揺れ
        {
            SetVibrationL(180.0f, 180.0f, 0.55f, 1);
            SetVibrationR(180.0f, 180.0f, 0.55f, 1);
        }
        else if (VibCountDamage < 6 + (m_VibDamageLoopCount - 1) * 4 && (VibCountDamage - 2) % 4 >= 2)    // 谷
        {
            SetVibrationL(180.0f, 180.0f, 0.15f, 1);
            SetVibrationR(180.0f, 180.0f, 0.15f, 1);
        }
        else if (VibCountDamage < 6 + (m_VibDamageLoopCount - 1) * 4 + 10)   // フェードアウト
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

	private int VibCountSkill = 0;
    // 必殺技時の振動処理
    private void VibrationSkillUpdate()
    {
        if (m_IsVibrationSkill == false) return;


        if (VibCountSkill < 1)  // フェードイン
        {
            SetVibrationL(180.0f, 180.0f, 0.2f, 1);
            SetVibrationR(180.0f, 180.0f, 0.2f, 1);
        }
        else if (VibCountSkill < 6) // 強い揺れ
        {
            SetVibrationL(180.0f, 180.0f, 0.7f, 1);
            SetVibrationR(180.0f, 180.0f, 0.7f, 1);
        }
        else if (VibCountSkill < m_VibSkillTime + 7)    // 電撃持続
        {
            SetVibrationL(60.0f, 120.0f, 0.9f, 1);
            SetVibrationR(60.0f, 120.0f, 0.9f, 1);
        }
        else if (VibCountSkill < m_VibSkillTime + 7 + 10)    // フェードアウト
        {
            SetVibrationL(60.0f, 120.0f, 0.2f, 1);
            SetVibrationR(60.0f, 120.0f, 0.2f, 1);
        }
        else
        {
            VibCountSkill = 0;
            m_IsVibrationSkill = false;
        }

        VibCountSkill ++;
    }

    private bool switchMode = false;
    private void ThunderModeUpdate()
    {
        // LRを同時押しした瞬間のみモード切り替え
        if (GetPressButtonL(JOYCON_BUTTON_LEFT.L) == true && GetPressButtonR(JOYCON_BUTTON_RIGHT.R) == true)
        {
            if (switchMode == false)
                m_IsThunderMode = !m_IsThunderMode;

            switchMode = true;
        }
        else
        {
            switchMode = false;
        }
    }



    private void Dbg()
    {
        Text DbgTextUI = m_DbgTextUI.GetComponent<Text>();

        DbgTextUI.text = m_DbgNumPunchi.ToString();


        if (GetTriggerButtonR(JOYCON_BUTTON_RIGHT.DPAD_A))
        {
            VibrationPunchiHitR();
        }
        if (GetTriggerButtonL(JOYCON_BUTTON_LEFT.DPAD_RIGHT))
        {
            VibrationPunchiHitL();
        }
        if (GetTriggerButtonL(JOYCON_BUTTON_LEFT.DPAD_LEFT))
        {
            VibrationPunchiShotL();
        }
        if (GetTriggerButtonL(JOYCON_BUTTON_LEFT.DPAD_UP))
        {
            VibrationDamage();
        }
        if (GetTriggerButtonL(JOYCON_BUTTON_LEFT.DPAD_DOWN))
        {
            VibrationSkill();
        }

        if (GetPunchL())
        {
            VibrationPunchiHitL();
        }

        if (GetPunchR())
        {
            VibrationPunchiHitR();
        }

        if (GetSpecialSkill())
        {
            VibrationSkill();
        }

        //if (GetPressButtonL(JOYCON_BUTTON_LEFT.L))
        //    Debug.Log("加速度Left：" + m_AccelBuff[0, 0] + "   回転速度Left：" + m_GyroBuff[0, 0] + "   回転の大きさLeft：" + m_GyroBuff[0, 0].magnitude);

        /*if (GetThunderMode())
        {
            Debug.Log("放電モード中！");
        }
        else
        {
            Debug.Log("放電モードしてない");
        }*/
    }
    
    private void KeyboardDebug()
    {
        if (m_KeybordMode == false)
            return;

        if (Input.GetKey((KeyCode)JOYCON_KEYBOARD.PUNCH_L) == true)
            m_IsPunch[0] = true;
        else
            m_IsPunch[0] = false;

        if (Input.GetKey((KeyCode)JOYCON_KEYBOARD.PUNCH_R) == true)
            m_IsPunch[1] = true;
        else
            m_IsPunch[1] = false;

        if (Input.GetKey((KeyCode)JOYCON_KEYBOARD.SPECIAL_SKILL) == true)
            m_IsSpecialSkill = true;
        else
            m_IsSpecialSkill = false;

        /*if (Input.GetKey((KeyCode)JOYCON_KEYBOARD.MODE_CHANGE) == true)
            m_IsSpecialSkill = true;
        else
            m_IsSpecialSkill = false;*/
        
    }

    

}

