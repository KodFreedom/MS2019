using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorStorage {

    private const int FrameWaitArray = 200;  // 加速度を保持するフレーム数
    private Vector3[,] m_AccelBuff = new Vector3[2, FrameWaitArray];   // 加速度を保持
    private Vector3[,] m_GyroBuff = new Vector3[2, FrameWaitArray];    // 回転速度を保持

    private InputManager m_InputManger = null;

    public SensorStorage(InputManager inputManager)
    {
        m_InputManger = inputManager;

        for (int i = 0; i < FrameWaitArray; i++)
        {
            m_AccelBuff[0, i] = new Vector3(0.0f, 0.0f, 0.0f);
            m_AccelBuff[1, i] = new Vector3(0.0f, 0.0f, 0.0f);
        }
    }

    ~SensorStorage()
    {

    }

    // フレーム保管の更新
    public void Update()
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
                m_AccelBuff[joyconType, 0] = m_InputManger.GetAccelL();
                m_GyroBuff[joyconType, 0] = m_InputManger.GetGyroL();
            }
            else
            {
                m_AccelBuff[joyconType, 0] = m_InputManger.GetAccelR();
                m_AccelBuff[joyconType, 0].y *= -1.0f;
                m_AccelBuff[joyconType, 0].z *= -1.0f;

                m_GyroBuff[joyconType, 0] = m_InputManger.GetGyroR();
                m_GyroBuff[joyconType, 0].y *= -1.0f;
                m_GyroBuff[joyconType, 0].z *= -1.0f;
            }
        }
    }

    public Vector3[,] GetAccelBuff()
    {
        return m_AccelBuff;
    }

    public Vector3[,] GetGyroBuff()
    {
        return m_GyroBuff;
    }
}
