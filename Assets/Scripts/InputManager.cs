using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    // キーボードでのデバッグ用
    public enum JOYCON_KEYBOARD
    {
        PUNCH_L = KeyCode.A,        // 左パンチのキー
        PUNCH_R = KeyCode.D,        // 右パンチのキー
        SPECIAL_SKILL = KeyCode.S,  // 必殺技のキー
        MODE_CHANGE = KeyCode.W     // 放電モード切替のキー
    }

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

    [SerializeField]
    private bool m_IsKeyboardMode = false;               // キーボードで操作できるようにするか

    [SerializeField]
    private bool m_IsDebug = true;                      // デバッグ中か


    //private static readonly Joycon.Button[] m_buttons =
    //    Enum.GetValues(typeof(Joycon.Button)) as Joycon.Button[];

    private List<Joycon> m_joycons;
    private Joycon m_joyconL;
    private Joycon m_joyconR;
    private Joycon.Button? m_pressedButtonL;
    private Joycon.Button? m_pressedButtonR;

    private bool[] m_IsKeyboardButton = new bool[4];    // キーボード操作時のパンチなどの可否を保管

    
    private SensorStorage m_SensorStorage;
    private PunchManager m_PunchManager;
    private SkillManager m_SkillManager;
    private SyncJoycon m_SyncJoycon;
    private ThunderModeChange m_ThunderModeChange;
    private VibrationManager m_VibrationManager;

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
        
        
        m_SensorStorage = new SensorStorage(this);
        m_PunchManager = new PunchManager(m_SensorStorage);
        m_SyncJoycon = new SyncJoycon(this);
        m_SkillManager = new SkillManager(m_SensorStorage, m_SyncJoycon);
        m_ThunderModeChange = new ThunderModeChange(this);
        m_VibrationManager = new VibrationManager(this);


        for (int i = 0; i < 4; i++)
            m_IsKeyboardButton[i] = false;
    }

    private void Update()
    {
        m_SensorStorage.Update();
        m_PunchManager.Update();
        m_SkillManager.Update();
        m_ThunderModeChange.Update();
        m_VibrationManager.Update();
        m_SyncJoycon.Update();

        KeyboardDebug();
        Dbg();
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
        if (m_IsKeyboardMode == true)
            return m_IsKeyboardButton[0];
        return m_PunchManager.GetIsPunchL();
    }
    public bool GetPunchR()
    {
        if (m_IsKeyboardMode == true)
            return m_IsKeyboardButton[1];
        return m_PunchManager.GetIsPunchR();
    }

    // 必殺技をしたか取得
    public bool GetSpecialSkill()
    {
        if (m_IsKeyboardMode == true)
            return m_IsKeyboardButton[2];
        return m_SkillManager.GetIsSkill();
    }

    // 放電モードかどうか取得
    public bool GetThunderMode()
    {
        /*if (m_IsKeyboardMode == true)
            return m_IsKeyboardButton[3];*/
        return m_ThunderModeChange.GetIsThunderMode();
    }

    // 放電モードの状態を変更
    public void SetThunderMode(bool mode)
    {
        m_ThunderModeChange.SetIsThunderMode(mode);
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
        m_VibrationManager.VibrationPunchiShotL(time);
    }
    public void VibrationPunchiShotR(int time = 5)
    {
        m_VibrationManager.VibrationPunchiShotR(time);
    }

    // パンチが当たった時の振動をする
    public void VibrationPunchiHitL()
    {
        m_VibrationManager.VibrationPunchiHitL();
    }
    public void VibrationPunchiHitR()
    {
        m_VibrationManager.VibrationPunchiHitR();
    }

    // ダメージを受けた時の振動をする
    public void VibrationDamage(int loopCount = 3)
    {
        m_VibrationManager.VibrationDamage(loopCount);
    }

    // 必殺技時の振動をする
    public void VibrationSkill(int time = 20)
    {
        m_VibrationManager.VibrationSkill(time);
    }

    // キーボードでのデバッグ
    private void KeyboardDebug()
    {
        if (m_IsKeyboardMode == false)
            return;

        if (Input.GetKey((KeyCode)JOYCON_KEYBOARD.PUNCH_L) == true)
            m_IsKeyboardButton[0] = true;
        else
            m_IsKeyboardButton[0] = false;

        if (Input.GetKey((KeyCode)JOYCON_KEYBOARD.PUNCH_R) == true)
            m_IsKeyboardButton[1] = true;
        else
            m_IsKeyboardButton[1] = false;

        if (Input.GetKey((KeyCode)JOYCON_KEYBOARD.SPECIAL_SKILL) == true)
            m_IsKeyboardButton[2] = true;
        else
            m_IsKeyboardButton[2] = false;

        if (Input.GetKeyDown((KeyCode)JOYCON_KEYBOARD.MODE_CHANGE) == true)
            //m_IsKeyboardButton[3] = !m_IsKeyboardButton[3];
            m_ThunderModeChange.SetIsThunderMode(!m_ThunderModeChange.GetIsThunderMode());

    }


    // デバッグ
    private void Dbg()
    {
        if (!m_IsDebug)
            return;

        /*
        // ボタンで振動テスト
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

        // アクションに振動を合わせてテスト
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
        */
        
        /*if (GetThunderMode())
        {
            Debug.Log("放電モード中！");
        }
        else
        {
            Debug.Log("放電モードしてない");
        }*/
    }

    

   

    

}

