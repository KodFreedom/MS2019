using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputManager : MonoBehaviour {

    public enum JOYCON_BUTTON
    {
        JOYCON_BUTTON_A = 0,
        JOYCON_BUTTON_B,
        JOYCON_BUTTON_X,
        JOYCON_BUTTON_Y
    }

    private static readonly Joycon.Button[] m_buttons =
        Enum.GetValues(typeof(Joycon.Button)) as Joycon.Button[];

    private List<Joycon> m_joycons;
    private Joycon m_joyconL;
    private Joycon m_joyconR;
    private Joycon.Button? m_pressedButtonL;
    private Joycon.Button? m_pressedButtonR;

    private void Start()
    {
        m_joycons = JoyconManager.Instance.j;

        if (m_joycons == null || m_joycons.Count <= 0) return;

        m_joyconL = m_joycons.Find(c => c.isLeft);
        m_joyconR = m_joycons.Find(c => !c.isLeft);
    }

    private void Update()
    {


    }

    public float GetTwistL()
    {
        var gyro = m_joyconL.GetGyro();

        //gyro.



        return 5.0f;
    }

    public float GetTwistR()
    {
        return 5.0f;
    }

    public bool GetShakeL()
    {
        return true;
    }

    public bool GetShakeR()
    {
        return true;
    }

    public bool GetPunchL()
    {
        return true;
    }

    public bool GetPunchR()
    {
        return true;
    }
}

