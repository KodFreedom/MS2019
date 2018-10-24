using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncJoycon {
    
    private int m_JoyconDelay = 5;              // 2つ目ジョイコンのずれフレーム

    private int m_delayFrame = 0;               // ずれ検知の作業用変数

    private InputManager m_InputManger;


    public SyncJoycon(InputManager inputManger)
    {
        m_InputManger = inputManger;

        m_JoyconDelay = 5;

        m_delayFrame = 0;
    }

    
    public void Update()
    {
        if (m_InputManger.GetTriggerButtonL(InputManager.JOYCON_BUTTON_LEFT.DPAD_RIGHT))
        {
            m_delayFrame = 0;
        }

        if (m_InputManger.GetPressButtonL(InputManager.JOYCON_BUTTON_LEFT.ZL) && m_InputManger.GetPressButtonR(InputManager.JOYCON_BUTTON_RIGHT.ZR))
        {
            if (m_InputManger.GetPressButtonL(InputManager.JOYCON_BUTTON_LEFT.DPAD_RIGHT))
            {
                if (m_InputManger.GetPressButtonR(InputManager.JOYCON_BUTTON_RIGHT.DPAD_A))
                {
                    m_JoyconDelay = m_delayFrame;
                    Debug.Log("右ジョイコンの遅延：" + m_delayFrame);
                }
                else
                {
                    m_delayFrame++;
                }
            }
        }
    }

    public int GetJoyconDelay()
    {
        return m_JoyconDelay;
    }
}
