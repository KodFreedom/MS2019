using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager {

    private bool m_IsSkill = false;  // 必殺技をしたか

    SensorStorage m_SensorStorage;
    SyncJoycon m_SyncJoycon;

    private Vector3[,] m_pAccelBuff;
    private Vector3[,] m_pGyroBuff;

    public SkillManager(SensorStorage sensorStorage, SyncJoycon syncJoycon)
    {
        m_IsSkill = false;

        m_SensorStorage = sensorStorage;
        m_SyncJoycon = syncJoycon;

        m_pAccelBuff = m_SensorStorage.GetAccelBuff();
        m_pGyroBuff = m_SensorStorage.GetGyroBuff();
    }
    
    
    public void Update()
    {
        m_IsSkill = false;


        int AccelCount = 0;
        int GyroCount = 0;

        int joyconDelay = m_SyncJoycon.GetJoyconDelay();


        for (int joyconType = 0; joyconType < 2; joyconType++)
        {

            // ひねっていたら除外
            int twistCount = 0;
            for (int i = 0 + (1 - joyconType) * joyconDelay; i < 10 + (1 - joyconType) * joyconDelay; i++)
            {
                if (m_pGyroBuff[joyconType, (1 - joyconType) * joyconDelay].magnitude > 19.0f)
                {
                    twistCount++;
                }
            }
            if (twistCount >= 6)
            {
                //Debug.Log("ひねっている(必殺技)");
                continue;
            }




            float minSpeed = 100.0f;   // 最小速度
            int AccelPlusCount = 0;
            for (int i = 0 + (1 - joyconType) * joyconDelay; i < 20 + (1 - joyconType) * joyconDelay; i++)
            {
                if (m_pAccelBuff[joyconType, i].x < 0.4f)
                {
                    AccelPlusCount++;
                }
                if (minSpeed > m_pAccelBuff[joyconType, i].x) minSpeed = m_pAccelBuff[joyconType, i].x;
            }

            bool IsAccelPlus = false;   // 振り下ろす操作の成功可否
            if (AccelPlusCount >= 12)
            {
                IsAccelPlus = true;
            }

            // 振り下ろしつつ、最後に止めたときに必殺技
            if (minSpeed - m_pAccelBuff[joyconType, (1 - joyconType) * joyconDelay].x < -1.4 && m_pAccelBuff[joyconType, (1 - joyconType) * joyconDelay].x > 0.2f && IsAccelPlus == true)
            {
                //float defference = minSpeed - m_pAccelBuff[joyconType, (1 - joyconType) * m_JoyconRDelay].x;
                //Debug.Log(" アクセル成功！" + " 速さ:" + m_pAccelBuff[joyconType, 0].x + " 最大の速さ:" + minSpeed + " その差" + defference);
                AccelCount++;
                //Debug.Log("必殺技加速成功カウントプラス");
            }

            for (int i = 0 + (1 - joyconType) * joyconDelay; i < 3 + (1 - joyconType) * joyconDelay; i++)
            {
                if (m_pGyroBuff[joyconType, (1 - joyconType) * joyconDelay].z > 3.0f)
                {
                    GyroCount++;
                    //Debug.Log("必殺技回転成功カウントプラス");
                    break;
                }
            }
        }
        if (AccelCount >= 2)
        {
            //Debug.Log("アクセル成功！" + m_pAccelBuff[0, m_JoyconRDelay] + "  " + m_AccelBuff[1, 0]);
            if (GyroCount >= 2)
            {
                m_IsSkill = true;
                Debug.Log("必殺技！");
                //Debug.Log(m_GyroBuff[0, m_JoyconRDelay] + "  " + m_pGyroBuff[1, 0]);

            }
            else
            {
                //Debug.Log("ジャイロカウントが足りない！" + m_GyroBuff[0, m_JoyconRDelay] + "  " + m_pGyroBuff[1, 0]);
            }


        }


    }

    public bool GetIsSkill()
    {
        return m_IsSkill;
    }
}
