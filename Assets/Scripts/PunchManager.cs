using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchManager {

    private bool[] m_IsPunch = new bool[2];    // パンチしたか

    SensorStorage m_SensorStorage;

    private Vector3[,] m_pAccelBuff;
    private Vector3[,] m_pGyroBuff;

    public PunchManager(SensorStorage sensorStorage) {
        m_IsPunch[0] = false;
        m_IsPunch[1] = false;

        m_SensorStorage = sensorStorage;

        m_pAccelBuff = m_SensorStorage.GetAccelBuff();
        m_pGyroBuff = m_SensorStorage.GetGyroBuff();

    }

    ~PunchManager()
    {

    }
    

    public void Update()
    {
        m_IsPunch[0] = false;
        m_IsPunch[1] = false;
        
        for (int joyconType = 0; joyconType < 2; joyconType++)
        {
            // ジャイロが大きければ処理しない
            int GyroCount = 0;
            for (int i = 0; i < 7; i++)
            {
                if (m_pGyroBuff[joyconType, i].magnitude > 12.0f)
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
            if (GyroCount >= 3)
            {
                //Debug.Log("ジャイロオーバー！");
                continue;
            }

            int AccelXCount = 0;
            for (int i = 0; i < 12; i++)
            {
                if (m_pAccelBuff[joyconType, i].x < -0.4)
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
                if (m_pAccelBuff[joyconType, i].y > 0.3f)
                {
                    AccelPlusCount++;
                    //Debug.Log("成功:" + AccelPlusCount + " 速さ:" + m_AccelBuff[joyconType, i].y);
                }
                if (maxSpeed < m_pAccelBuff[joyconType, i].y) maxSpeed = m_pAccelBuff[joyconType, i].y;
            }

            bool IsAccelPlus = false;   // 突き出す操作の成功可否
            if (AccelPlusCount >= 5)
            {
                IsAccelPlus = true;
            }

            if (maxSpeed < 0.0f) continue;

            // 突き出しつつ、最後に止めたときにパンチ
            if (maxSpeed - m_pAccelBuff[joyconType, 0].y > 2.0f && m_pAccelBuff[joyconType, 0].y < -0.8f && IsAccelPlus == true)
            {
                m_IsPunch[joyconType] = true;

                float defference = maxSpeed - m_pAccelBuff[joyconType, 0].y;
                Debug.Log("パンチした！" + " 速さ:" + m_pAccelBuff[joyconType, 0].y + " 最大の速さ:" + maxSpeed + " その差" + defference);
                //Debug.Log("→回転速度：" + m_GyroBuff[joyconType, 0].magnitude + "Z回転：" + m_GyroBuff[joyconType, 0].z);
            }
        }
    }

    public bool GetIsPunchL()
    {
        return m_IsPunch[0];
    }

    public bool GetIsPunchR()
    {
        return m_IsPunch[1];
    }
}
