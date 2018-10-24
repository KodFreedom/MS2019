using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderModeChange {

    private bool m_IsThunderMode = false;       // 放電モード中か

    private bool switchMode = false;

    private InputManager m_InputManager;

    public ThunderModeChange(InputManager inputManager)
    {
        m_InputManager = inputManager;

        m_IsThunderMode = false;

        switchMode = false;
    }

    ~ThunderModeChange()
    {

    }

    public bool GetIsThunderMode()
    {
        return m_IsThunderMode;
    }

    public void SetIsThunderMode(bool mode)
    {
        m_IsThunderMode = mode;
    }

    public void Update()
    {
        // LRを同時押しした瞬間のみモード切り替え
        if (m_InputManager.GetPressButtonL(InputManager.JOYCON_BUTTON_LEFT.L) == true && m_InputManager.GetPressButtonR(InputManager.JOYCON_BUTTON_RIGHT.R) == true)
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
}
