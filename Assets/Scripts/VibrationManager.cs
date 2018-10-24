using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrationManager {

    InputManager m_InputManager;

    private bool[] m_IsVibPunchiHit = new bool[2];   // パンチヒットの振動をならしているか
    private bool[] m_IsVibPunchiShot = new bool[2];   // パンチした時の振動をならしているか
    private bool m_IsVibDamage = false;   // ダメージ受けた時の振動をならしているか
    private bool m_IsVibSkill = false;   // ダメージ受けた時の振動をならしているか

    private int m_VibDamageLoopCount = 3;       // ダメージ時の振動の起伏回数
    private int m_VibSkillTime = 20;            // 必殺技時の振動の長さ
    private int m_VibPunchShotTime = 5;         // パンチ時の振動の長さ

    private int[] VibCountPunchShot = new int[2];   // パンチの振動中のフレーム数
    private int[] VibCountPunchHit = new int[2];    // パンチヒットの振動中のフレーム数
    private int VibCountDamage = 0;                 // ダメージの振動中のフレーム数
    private int VibCountSkill = 0;                  // 必殺技の振動中のフレーム数

    public VibrationManager(InputManager inputManager)
    {
        m_InputManager = inputManager;

        m_IsVibPunchiHit[0] = false;
        m_IsVibPunchiHit[1] = false;

        m_IsVibDamage = false;
        m_IsVibSkill = false;

        m_VibDamageLoopCount = 3;
        m_VibSkillTime = 20;
        m_VibPunchShotTime = 5;


        VibCountPunchShot[0] = 0;
        VibCountPunchShot[1] = 0;

        VibCountPunchHit[0] = 0;
        VibCountPunchHit[1] = 0;

        VibCountDamage = 0;
        VibCountSkill = 0;
    }

    public void Update()
    {
        PunchiHitUpdate();
        DamageUpdate();
        SkillUpdate();
        PunchiShotUpdate();
    }

    // パンチの振動をする
    public void VibrationPunchiShotL(int time = 5)
    {
        m_VibPunchShotTime = time;
        m_IsVibPunchiShot[0] = true;
    }
    public void VibrationPunchiShotR(int time = 5)
    {
        m_VibPunchShotTime = time;
        m_IsVibPunchiShot[0] = true;
    }

    // パンチが当たった時の振動をする
    public void VibrationPunchiHitL()
    {
        m_IsVibPunchiHit[0] = true;
    }
    public void VibrationPunchiHitR()
    {
        m_IsVibPunchiHit[1] = true;
    }

    // ダメージを受けた時の振動をする
    public void VibrationDamage(int loopCount = 3)
    {
        m_IsVibDamage = true;
        m_VibDamageLoopCount = loopCount;
    }

    // 必殺技時の振動をする
    public void VibrationSkill(int time = 20)
    {
        m_IsVibSkill = true;
        m_VibSkillTime = time;
    }


    // パンチヒット時の振動処理
    private void PunchiShotUpdate()
    {
        for (int i = 0; i < 2; i++)
        {
            if (m_IsVibPunchiShot[i] == false) continue;

            bool isLeft = false;
            if (i == 0) isLeft = true;
            else isLeft = false;

            if (VibCountPunchShot[i] < m_VibPunchShotTime) // フェードアウト
            {
                m_InputManager.SetVibration(isLeft, 300.0f, 300.0f, 0.7f, 1);
            }
            else
            {
                VibCountPunchShot[i] = 0;
                m_IsVibPunchiShot[i] = false;
            }

            VibCountPunchShot[i]++;
        }
    }

    
    // パンチヒット時の振動処理
    private void PunchiHitUpdate()
    {
        for (int i = 0; i < 2; i++)
        {
            if (m_IsVibPunchiHit[i] == false) continue;

            bool isLeft = false;
            if (i == 0) isLeft = true;
            else isLeft = false;

            if (VibCountPunchHit[i] < 2)   // フェードイン
            {
                m_InputManager.SetVibration(isLeft, 180.0f, 180.0f, 0.2f, 1);
            }
            else if (VibCountPunchHit[i] < 6)  // 拳に来る振動
            {
                m_InputManager.SetVibration(isLeft, 180.0f, 180.0f, 0.45f, 1);
            }
            else if (VibCountPunchHit[i] < 10) // 振動引き
            {
                m_InputManager.SetVibration(isLeft, 180.0f, 180.0f, 0.2f, 1);
            }
            else if (VibCountPunchHit[i] < 12) // フェードアウト
            {
                m_InputManager.SetVibration(isLeft, 180.0f, 180.0f, 0.1f, 1);
            }
            else
            {
                VibCountPunchHit[i] = 0;
                m_IsVibPunchiHit[i] = false;
            }

            VibCountPunchHit[i]++;
        }
    }

    
    // ダメージを受けた時の振動処理
    private void DamageUpdate()
    {
        if (m_IsVibDamage == false) return;


        if (VibCountDamage < 2)     // フェードイン
        {
            m_InputManager.SetVibrationL(180.0f, 180.0f, 0.2f, 1);
            m_InputManager.SetVibrationR(180.0f, 180.0f, 0.2f, 1);
        }
        else if (VibCountDamage < 4 + (m_VibDamageLoopCount - 1) * 4 && (VibCountDamage - 2) % 4 < 2)    // 強い揺れ
        {
            m_InputManager.SetVibrationL(180.0f, 180.0f, 0.55f, 1);
            m_InputManager.SetVibrationR(180.0f, 180.0f, 0.55f, 1);
        }
        else if (VibCountDamage < 6 + (m_VibDamageLoopCount - 1) * 4 && (VibCountDamage - 2) % 4 >= 2)    // 谷
        {
            m_InputManager.SetVibrationL(180.0f, 180.0f, 0.15f, 1);
            m_InputManager.SetVibrationR(180.0f, 180.0f, 0.15f, 1);
        }
        else if (VibCountDamage < 6 + (m_VibDamageLoopCount - 1) * 4 + 10)   // フェードアウト
        {
            m_InputManager.SetVibrationL(180.0f, 180.0f, 0.1f, 1);
            m_InputManager.SetVibrationR(180.0f, 180.0f, 0.1f, 1);
        }
        else
        {
            VibCountDamage = 0;
            m_IsVibDamage = false;
        }

        VibCountDamage++;
    }

    
    // 必殺技時の振動処理
    private void SkillUpdate()
    {
        if (m_IsVibSkill == false) return;


        if (VibCountSkill < 1)  // フェードイン
        {
            m_InputManager.SetVibrationL(180.0f, 180.0f, 0.2f, 1);
            m_InputManager.SetVibrationR(180.0f, 180.0f, 0.2f, 1);
        }
        else if (VibCountSkill < 6) // 強い揺れ
        {
            m_InputManager.SetVibrationL(180.0f, 180.0f, 0.7f, 1);
            m_InputManager.SetVibrationR(180.0f, 180.0f, 0.7f, 1);
        }
        else if (VibCountSkill < m_VibSkillTime + 7)    // 電撃持続
        {
            m_InputManager.SetVibrationL(60.0f, 120.0f, 0.9f, 1);
            m_InputManager.SetVibrationR(60.0f, 120.0f, 0.9f, 1);
        }
        else if (VibCountSkill < m_VibSkillTime + 7 + 10)    // フェードアウト
        {
            m_InputManager.SetVibrationL(60.0f, 120.0f, 0.2f, 1);
            m_InputManager.SetVibrationR(60.0f, 120.0f, 0.2f, 1);
        }
        else
        {
            VibCountSkill = 0;
            m_IsVibSkill = false;
        }

        VibCountSkill++;
    }
}
