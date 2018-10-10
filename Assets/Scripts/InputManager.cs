using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    // ボタンの種類列挙
    public enum JOYCON_BUTTON
    {
        DPAD_DOWN = 0,  DPAD_B = 0,
        DPAD_RIGHT = 1, DPAD_A = 1,
        DPAD_LEFT = 2,  DPAD_Y = 2,
        DPAD_UP = 3,    DPAD_X = 3,
        SL = 4,
        SR = 5,
        MINUS = 6,
        HOME = 7,
        PLUS = 8,
        CAPTURE = 9,
        STICK = 10,
        L = 11, R = 11,
        ZL = 12, ZR = 12,
    }

    private static readonly Joycon.Button[] m_buttons =
        Enum.GetValues(typeof(Joycon.Button)) as Joycon.Button[];

    private List<Joycon> m_joycons;
    private Joycon m_joyconL;
    private Joycon m_joyconR;
    private Joycon.Button? m_pressedButtonL;
    private Joycon.Button? m_pressedButtonR;


    private const int PunchWaitArray = 200;  // 加速度を保持するフレーム
    private Vector3[] m_PunchiVec = new Vector3[PunchWaitArray];    // 加速度を保持
    private bool m_IsPunch;    // パンチしたか

    public GameObject m_DbgTextUI;
    public int m_DbgNumPunchi;

    

    private void Start()
    {
        m_joycons = JoyconManager.Instance.j;

        if (m_joycons == null || m_joycons.Count <= 0) return;

        m_joyconL = m_joycons.Find(c => c.isLeft);
        m_joyconR = m_joycons.Find(c => !c.isLeft);


        for (int i = 0; i < PunchWaitArray; i++)
            m_PunchiVec[0] = new Vector3(0.0f, 0.0f, 0.0f);

        m_IsPunch = false;

        m_DbgNumPunchi = 0;

    }

    private void Update()
    {
        Punch();
    }

    // ひねりの回転の大きさを取得（ジョイコンL）
    public float GetTwistL()
    {
        var gyro = m_joyconL.GetGyro();

        return gyro.magnitude;
    }

    public float GetTwistR()
    {
        var gyro = m_joyconR.GetGyro();

        return gyro.magnitude;
    }

    // 振った大きさを取得（ジョイコンL）
    public float GetShakeL()
    {
        var accel = m_joyconL.GetAccel();

        return accel.magnitude;
    }
    
    public float GetShakeR()
    {
        var accel = m_joyconR.GetAccel();

        return accel.magnitude;
    }

    // 傾きを取得（ジョイコンL）
    public Quaternion GetSlopeL()
    {
        var slope = m_joyconL.GetVector();

        return slope;
    }

    public Quaternion GetSlopeR()
    {
        var slope = m_joyconR.GetVector();

        return slope;
    }

    // パンチしたか取得（ジョイコンL）
    public bool GetPunchL()
    {
        return m_IsPunch;
    }

    public bool GetPunchR()
    {
        return true;
    }

    // ボタンの取得（ジョイコンL）
    public bool GetPressButtonL(JOYCON_BUTTON button)
    {
        return m_joyconL.GetButton((Joycon.Button)button);
    }

    public bool GetTriggerButtonL(JOYCON_BUTTON button)
    {
        return m_joyconL.GetButtonDown((Joycon.Button)button);
    }

    public bool GetReleaseButtonL(JOYCON_BUTTON button)
    {
        return m_joyconL.GetButtonUp((Joycon.Button)button);
    }
    
    public bool GetPressButtonR(JOYCON_BUTTON button)
    {
        return m_joyconR.GetButton((Joycon.Button)button);
    }

    public bool GetTriggerButtonR(JOYCON_BUTTON button)
    {
        return m_joyconR.GetButtonDown((Joycon.Button)button);
    }

    public bool GetReleaseButtonR(JOYCON_BUTTON button)
    {
        return m_joyconR.GetButtonUp((Joycon.Button)button);
    }

    // 振動させる（ジョイコンL）
    public void SetVibrationL(float width, float height, float power, int time)
    {
        m_joyconL.SetRumble(width, height, power, time);
    }

    public void SetVibrationR(float width, float height, float power, int time)
    {
        m_joyconR.SetRumble(width, height, power, time);
    }

    public Vector3 GetGyroL()
    {
        return m_joyconL.GetGyro();
    }

    public Vector3 GetGyroR()
    {
        return m_joyconR.GetGyro();
    }


    private void Punch()
    {
        Text DbgTextUI = m_DbgTextUI.GetComponent<Text>();

        //if (GetPressButtonL(JOYCON_BUTTON.L))
        //Debug.Log(m_joyconL.GetVector().eulerAngles );
        
        
        /*float spoolUp = m_joyconL.GetVector().eulerAngles.y;    // 上を向いているとき、spoorUpは180近くになる(+-は初期角度による)
        Debug.Log(spoolUp);
        if (spoolUp < 160.0f || spoolUp > 200.0f)
        {
            Debug.Log("傾きが大きい");
            return;
        }*/

        if(GetTwistL() > 5.0f)
        {
            Debug.Log("ジャイロが大きい");
            return;
        }


        // 保管フレームの更新
        for (int i = PunchWaitArray - 1; i >= 1; i--)
        {
            m_PunchiVec[i] = m_PunchiVec[i - 1];
        }
        m_PunchiVec[0] = m_joyconL.GetAccel();


        



        /*bool turnPunchFlag = false;
        for (int i = 0; i < 15; i++)
        {
            if (m_PunchiVec[i].y < -1.0f)
            {
                turnPunchFlag = true;
                break;
            }
        }
        if (turnPunchFlag == false)
        {
            int punchFrame = 7;
            int punchCount = 0;
            for (int i = 0; i < punchFrame; i++)
            {
                if (m_PunchiVec[i].y > 0.3f)
                {
                    punchCount++;
                    //Debug.Log("成功:" + punchCount + " 速さ:" + m_PunchiVec[i].y + " しきい値:" + "なし");
                }
            }
            if (punchCount >= punchFrame)
            {
                m_IsPunch = true;
                m_DbgNumPunchi++;
                SetVibrationL(50.0f, 20.0f, 10.0f, 1);
            }
        }*/

        /*int minusPanchCount = 0;
        for (int i = 0; i < 20; i++)
        {
            if (m_PunchiVec[i].y < -0.3f)
            {
                minusPanchCount++;
            }
        }
        if (minusPanchCount >= 15)
        {
            Debug.Log("逆パンチ");
            return;
        }*/



        float maxSpeed = -100.0f;
        int AccelPlusCount = 0;
        for (int i = 0; i < 7; i++)
        {
            if (m_PunchiVec[i].y > 0.3f)
            {
                AccelPlusCount++;
                Debug.Log("成功:" + AccelPlusCount + " 速さ:" + m_PunchiVec[i].y);
            }
            if (maxSpeed < m_PunchiVec[i].y) maxSpeed = m_PunchiVec[i].y;
        }

        bool IsAccelPlus = false;
        if (AccelPlusCount >= 3)
        {
            IsAccelPlus = true;
        }

        if (maxSpeed < 0.0f) return;

        if(maxSpeed - m_PunchiVec[0].y > 2.0f && IsAccelPlus == true && m_PunchiVec[0].y < -0.8f)
        {
            m_IsPunch = true;
            m_DbgNumPunchi++;
            SetVibrationL(50.0f, 20.0f, 1.0f, 5);

            float defference = maxSpeed - m_PunchiVec[0].y;
            Debug.Log("パンチした！" + " 速さ:" + m_PunchiVec[0].y + " 最大の速さ:" + maxSpeed + " その差" + defference);
        }



        // 瞬間的に突き出せばパンチとする
        /*int speedPunchCount = 0;
        for(int i = 0; i < 5; i++)
        {
            if(m_PunchiVec[i].magnitude > 1.80f)
            {
                speedPunchCount++;
            }
        }
        if(speedPunchCount >= 3)
        {
            m_IsPunch = true;
            m_DbgNumPunchi++;
            SetVibrationL(50.0f, 20.0f, 10.0f, 1);
        }*/


        // 長く前に突き出せばパンチとする
        /*for (int i = 0; i < 15; i++)
        {
            if (m_PunchiVec[i].y > 0.3f)
            {
                AccelPlusCount++;
            }
        }
        if(AccelPlusCount >= 15)
        {
            m_IsPunch = true;
            m_DbgNumPunchi++;
            SetVibrationL(50.0f, 20.0f, 10.0f, 1);
        }*/


        DbgTextUI.text = m_DbgNumPunchi.ToString();


        //Debug.Log(GetShakeL() + " : " + m_joyconL.GetAccel());
    }
}

