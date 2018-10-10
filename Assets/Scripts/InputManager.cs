﻿using System;
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
    private const int PunchWaitArray = 200;  // 加速度を保持するフレーム
    private Vector3[,] m_PunchiVec = new Vector3[JoyconType, PunchWaitArray];    // 加速度を保持
    private bool[] m_IsPunch = new bool[JoyconType];    // パンチしたか

    private bool IsDebug = true;
    public GameObject m_DbgTextUI;
    public int m_DbgNumPunchi;



    private void Start()
    {
        m_joycons = JoyconManager.Instance.j;

        if (m_joycons == null || m_joycons.Count <= 0) return;

        m_joyconL = m_joycons.Find(c => c.isLeft);
        m_joyconR = m_joycons.Find(c => !c.isLeft);


        for (int i = 0; i < PunchWaitArray; i++)
        {
            m_PunchiVec[0, i] = new Vector3(0.0f, 0.0f, 0.0f);
            m_PunchiVec[1, i] = new Vector3(0.0f, 0.0f, 0.0f);
        }

        m_IsPunch[0] = false;
        m_IsPunch[1] = false;
        
        m_DbgNumPunchi = 0;

    }

    private void Update()
    {
        Punch();
        
        if(IsDebug) Dbg();
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

    /*public Vector3 GetMotionL(JOYCON_MOTION motion)
    {
        switch(motion)
        {
            case JOYCON_MOTION.ACCLEL:
                return m_joyconL.GetAccel();

            case JOYCON_MOTION.GYRO:
                return m_joyconL.GetGyro();

            case JOYCON_MOTION.SLOPE:
                return m_joyconL.GetVector().eulerAngles;
        }

        return new Vector3(0.0f, 0.0f, 0.0f);
    }*/

    // 後でクラス化したい
    private void Punch()
    {
        

        for (int joyconType = 0; joyconType < 2; joyconType++)
        {

            //if (GetPressButtonL(JOYCON_BUTTON.L))
            //Debug.Log(m_joyconL.GetVector().eulerAngles );


            /*float spoolUp = m_joyconL.GetVector().eulerAngles.y;    // 上を向いているとき、spoorUpは180近くになる(+-は初期角度による)
            Debug.Log(spoolUp);
            if (spoolUp < 160.0f || spoolUp > 200.0f)
            {
                Debug.Log("傾きが大きい");
                return;
            }*/

            // ジャイロが大きければ処理しない
            if (joyconType == 0)
            {
                if (GetTwistL() > 5.0f)
                {
                    Debug.Log("ジャイロが大きい");
                    return;
                }
            }
            else
            {
                if (GetTwistR() > 5.0f)
                {
                    Debug.Log("ジャイロが大きい");
                    return;
                }
            }


            // 加速度フレーム保管の更新
            for (int i = PunchWaitArray - 1; i >= 1; i--)
            {
                m_PunchiVec[joyconType, i] = m_PunchiVec[joyconType, i - 1];
            }
            if (joyconType == 0)
            {
                m_PunchiVec[joyconType, 0] = m_joyconL.GetAccel();
            }
            else
            {
                m_PunchiVec[joyconType, 0] = m_joyconR.GetAccel();
                m_PunchiVec[joyconType, 0].y *= -1.0f;
                m_PunchiVec[joyconType, 0].z *= -1.0f;
            }





            float maxSpeed = -100.0f;   // 最大速度
            int AccelPlusCount = 0;     // 前に加速したフレーム数
            for (int i = 0; i < 7; i++)
            {
                if (m_PunchiVec[joyconType, i].y > 0.3f)
                {
                    AccelPlusCount++;
                    Debug.Log("成功:" + AccelPlusCount + " 速さ:" + m_PunchiVec[joyconType, i].y);
                }
                if (maxSpeed < m_PunchiVec[joyconType, i].y) maxSpeed = m_PunchiVec[joyconType, i].y;
            }

            bool IsAccelPlus = false;   // 突き出す操作の成功可否
            if (AccelPlusCount >= 3)
            {
                IsAccelPlus = true;
            }

            if (maxSpeed < 0.0f) return;

            // 突き出しつつ、最後に止めたときにパンチ
            if (maxSpeed - m_PunchiVec[joyconType, 0].y > 2.0f && m_PunchiVec[joyconType, 0].y < -0.8f && IsAccelPlus == true)
            {
                m_IsPunch[joyconType] = true;
                m_DbgNumPunchi++;
                if(joyconType == 0) SetVibrationL(150.0f, 120.0f, 0.6f, 10);
                else SetVibrationR(150.0f, 120.0f, 0.6f, 10);

                float defference = maxSpeed - m_PunchiVec[joyconType, 0].y;
                Debug.Log("パンチした！" + " 速さ:" + m_PunchiVec[joyconType, 0].y + " 最大の速さ:" + maxSpeed + " その差" + defference);
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


            
        }

    }
    
    private void Dbg()
    {
        Text DbgTextUI = m_DbgTextUI.GetComponent<Text>();

        DbgTextUI.text = m_DbgNumPunchi.ToString();
    }
}

