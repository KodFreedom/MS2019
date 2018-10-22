using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElectricalValue : MonoBehaviour {

    public GameObject InputManagerObject;

    public GameObject TextUI;

    private float Energy;

    private Vector2 testVib = new Vector2(150.0f, 120.0f);
    private float powerVib = 0.6f;

    private int VibTime = 10;

    void Start () {
        Energy = 0.0f;

    }
	
	void Update () {
        InputManager inputManager = InputManagerObject.GetComponent<InputManager>();

        float TotalTwist = 0.0f;
        float twist;
        twist = inputManager.GetTwistL();
        if (twist > 10.0f)
            TotalTwist += twist;

        twist = inputManager.GetTwistR();
        if (twist > 10.0f)
            TotalTwist += twist;

        TotalTwist /= 50.0f;
        

        Energy += TotalTwist;

        

        Text textUI = TextUI.GetComponent<Text>();

        textUI.text = Energy.ToString();


        if (Input.GetKeyDown(KeyCode.Z))
        {
            Energy -= 500.0f;
        }

        if(inputManager.GetPressButtonL(InputManager.JOYCON_BUTTON_LEFT.L))
        {
            Energy -= 1.0f;
        }

        if (inputManager.GetPressButtonL(InputManager.JOYCON_BUTTON_LEFT.DPAD_LEFT))
        {
            Energy -= 1.0f;
        }

        if (inputManager.GetPressButtonR(InputManager.JOYCON_BUTTON_RIGHT.ZR))
        {
            Energy -= 1.0f;
        }


        /*
        if (inputManager.GetPressButtonL(InputManager.JOYCON_BUTTON_LEFT.DPAD_UP))
        {
            testVib.y -= 1.0f;
            inputManager.SetVibrationL(testVib.x, testVib.y, powerVib, VibTime);
        }

        if (inputManager.GetPressButtonL(InputManager.JOYCON_BUTTON_LEFT.DPAD_LEFT))
        {
            testVib.x -= 1.0f;
            inputManager.SetVibrationL(testVib.x, testVib.y, powerVib, VibTime);
        }

        if (inputManager.GetPressButtonL(InputManager.JOYCON_BUTTON_LEFT.DPAD_DOWN))
        {
            testVib.y += 1.0f;
            inputManager.SetVibrationL(testVib.x, testVib.y, powerVib, VibTime);
        }

        if (inputManager.GetPressButtonL(InputManager.JOYCON_BUTTON_LEFT.DPAD_RIGHT))
        {
            testVib.x += 1.0f;
            inputManager.SetVibrationL(testVib.x, testVib.y, powerVib, VibTime);
        }

        if (inputManager.GetPressButtonL(InputManager.JOYCON_BUTTON_LEFT.SL))
        {
            powerVib -= 0.01f;
            inputManager.SetVibrationL(testVib.x, testVib.y, powerVib, VibTime);
        }

        if (inputManager.GetPressButtonL(InputManager.JOYCON_BUTTON_LEFT.SR))
        {
            powerVib += 0.01f;
            inputManager.SetVibrationL(testVib.x, testVib.y, powerVib, VibTime);
        }

        if (inputManager.GetPressButtonL(InputManager.JOYCON_BUTTON_LEFT.ZL))
        {
            inputManager.SetVibrationL(testVib.x, testVib.y, powerVib, VibTime);
        }

        if (inputManager.GetTriggerButtonL(InputManager.JOYCON_BUTTON_LEFT.L))
        {
            inputManager.SetVibrationL(testVib.x, testVib.y, powerVib, VibTime);
        }


        if (inputManager.GetPressButtonL(InputManager.JOYCON_BUTTON_LEFT.MINUS))
        {
            VibTime++;
            inputManager.SetVibrationL(testVib.x, testVib.y, powerVib, VibTime);
        }

        if (inputManager.GetPressButtonL(InputManager.JOYCON_BUTTON_LEFT.STICK))
        {
            VibTime--;
            inputManager.SetVibrationL(testVib.x, testVib.y, powerVib, VibTime);
        }


        Debug.Log("振れ幅：" + testVib + "強さ：" + powerVib + "長さ：" + VibTime);
        */
        

        /*if (inputManager.GetPunchL())
        {
            Energy -= 5.0f;
        }

        if (inputManager.GetPunchR())
        {
            Energy -= 5.0f;
        }*/
    }

    public float GetEnergy()
    {
        return Energy;
    }
}
