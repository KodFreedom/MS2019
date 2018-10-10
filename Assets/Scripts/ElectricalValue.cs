using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElectricalValue : MonoBehaviour {

    public GameObject InputManagerObject;

    public GameObject TextUI;

    private float Energy;

    private Vector2 testVib = new Vector2(0.0f, 0.0f);
    private float powerVib = 0.0f;

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



        if (inputManager.GetPressButtonL(InputManager.JOYCON_BUTTON_LEFT.DPAD_UP))
        {
            testVib.y -= 1.0f;
            inputManager.SetVibrationL(testVib.x, testVib.y, powerVib, 10);
        }

        if (inputManager.GetPressButtonL(InputManager.JOYCON_BUTTON_LEFT.DPAD_LEFT))
        {
            testVib.x -= 1.0f;
            inputManager.SetVibrationL(testVib.x, testVib.y, powerVib, 10);
        }

        if (inputManager.GetPressButtonL(InputManager.JOYCON_BUTTON_LEFT.DPAD_DOWN))
        {
            testVib.y += 1.0f;
            inputManager.SetVibrationL(testVib.x, testVib.y, powerVib, 10);
        }

        if (inputManager.GetPressButtonL(InputManager.JOYCON_BUTTON_LEFT.DPAD_RIGHT))
        {
            testVib.x += 1.0f;
            inputManager.SetVibrationL(testVib.x, testVib.y, powerVib, 10);
        }

        if (inputManager.GetPressButtonL(InputManager.JOYCON_BUTTON_LEFT.SL))
        {
            powerVib -= 0.01f;
            inputManager.SetVibrationL(testVib.x, testVib.y, powerVib, 10);
        }

        if (inputManager.GetPressButtonL(InputManager.JOYCON_BUTTON_LEFT.SR))
        {
            powerVib += 0.01f;
            inputManager.SetVibrationL(testVib.x, testVib.y, powerVib, 10);
        }

        if (inputManager.GetPressButtonL(InputManager.JOYCON_BUTTON_LEFT.ZL))
        {
            inputManager.SetVibrationL(testVib.x, testVib.y, powerVib, 10);
        }

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
