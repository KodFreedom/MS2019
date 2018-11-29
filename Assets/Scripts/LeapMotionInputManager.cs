using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeapMotionInputManager : MonoBehaviour
{
    public static LeapMotionInputManager Instance { get; private set; }

    public enum HandSide
    {
        Left,
        Right,
    }

    private struct PunchInfo
    {
        public float[] PreviousZs;
        public int FrameCounter;
        public bool Succeeded;
    }

    private struct UltraInfo
    {
        public float[,] PreviusYs;
        public int FrameCounter;
        public bool Succeeded;
    }

    [System.Serializable]
    private struct HandInfo
    {
        public Transform Palm;
        public Transform Forearm;
        public float PreviousForearmEulerX;
        public PunchInfo Punch;
    }

    private static readonly int kPunchCheckFrameNumber = 10;
    private static readonly int kUltraCheckFrameNumber = 10;
    [SerializeField] HandInfo[] Hands = new HandInfo[2];
    [SerializeField] float PunchCheckBias = 0.001f;
    [SerializeField] float UltraCheckBias = 0.001f;
    [SerializeField, Range(0, 10)] int PunchSuccessFrame = 8;
    [SerializeField, Range(0, 10)] int UltraSuccessFrame = 7;
    private UltraInfo Ultra = new UltraInfo();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitPunch(HandSide.Left);
        InitPunch(HandSide.Right);
        InitUltra();
    }

    public bool GetPunch(HandSide side)
    {
        return Hands[(int)side].Punch.Succeeded;
    }

    public bool GetUltra()
    {
        return Ultra.Succeeded;
    }

    public float GetForearmRotate(HandSide side)
    {
        return -Hands[(int)side].Forearm.localEulerAngles.x;
    }

    public float GetForearmRotateAcc(HandSide side)
    {
        return -Hands[(int)side].Forearm.localEulerAngles.x + Hands[(int)side].PreviousForearmEulerX;
    }

    private void LateUpdate()
    {
        // Punch
        UpdatePunch(HandSide.Left);
        UpdatePunch(HandSide.Right);

        // Charge
        Hands[(int)HandSide.Left].PreviousForearmEulerX = Hands[(int)HandSide.Left].Forearm.localEulerAngles.x;
        Hands[(int)HandSide.Right].PreviousForearmEulerX = Hands[(int)HandSide.Right].Forearm.localEulerAngles.x;

        // Ultra
        UpdateUltra();
    }

    private void InitPunch(HandSide side)
    {
        Hands[(int)side].Punch.PreviousZs = new float[kPunchCheckFrameNumber];
    }

    private void UpdatePunch(HandSide side)
    {
        Hands[(int)side].Punch.Succeeded = false;

        // 前フレームを保存した所の次の位置からループする
        int start_frame = Hands[(int)side].Punch.FrameCounter % kPunchCheckFrameNumber;
        int accel_plus_count = 0;
        for (int frame_counter = 0; frame_counter < kPunchCheckFrameNumber; ++frame_counter)
        {
            int previous_frame = (start_frame + frame_counter - 1 + kPunchCheckFrameNumber) % kPunchCheckFrameNumber;
            int current_frame = (start_frame + frame_counter) % kPunchCheckFrameNumber;
            if(Hands[(int)side].Punch.PreviousZs[current_frame] - PunchCheckBias
                > Hands[(int)side].Punch.PreviousZs[previous_frame])
            {
                accel_plus_count++;
            }
        }

        // 突き出す操作の成功
        if (accel_plus_count >= PunchSuccessFrame)
        {
            // 最後に止めたときにパンチ
            int prevois_frame = (Hands[(int)side].Punch.FrameCounter - 1 + kPunchCheckFrameNumber) % kPunchCheckFrameNumber;
            if (Hands[(int)side].Forearm.localPosition.z <= Hands[(int)side].Punch.PreviousZs[prevois_frame])
            {
                Hands[(int)side].Punch.Succeeded = true;
                Debug.Log(side + "Punch");
            }
        }

        // 値を保存する
        Hands[(int)side].Punch.PreviousZs[Hands[(int)side].Punch.FrameCounter] = Hands[(int)side].Forearm.localPosition.z;
        Hands[(int)side].Punch.FrameCounter = (Hands[(int)side].Punch.FrameCounter + 1) % kPunchCheckFrameNumber;
    }

    private void InitUltra()
    {
        Ultra.PreviusYs = new float[2,kUltraCheckFrameNumber];
    }

    private void UpdateUltra()
    {
        Ultra.Succeeded = false;

        // 前フレームを保存した所の次の位置からループする
        int start_frame = Ultra.FrameCounter % kUltraCheckFrameNumber;
        int[] accel_minus_count = new int[2];
        for(int side_counter = 0; side_counter < 2; ++side_counter)
        {
            accel_minus_count[side_counter] = 0;

            for (int frame_counter = 0; frame_counter < kUltraCheckFrameNumber; ++frame_counter)
            {
                int previous_frame = (start_frame + frame_counter - 1 + kUltraCheckFrameNumber) % kUltraCheckFrameNumber;
                int current_frame = (start_frame + frame_counter) % kUltraCheckFrameNumber;
                if (Ultra.PreviusYs[side_counter, current_frame] + UltraCheckBias
                    < Ultra.PreviusYs[side_counter, previous_frame])
                {
                    accel_minus_count[side_counter]++;
                }
            }
        }


        // 突き出す操作の成功
        if (accel_minus_count[0] >= UltraSuccessFrame && accel_minus_count[1] >= UltraSuccessFrame)
        {
            // 最後に止めたときにパンチ
            int prevois_frame = (Ultra.FrameCounter - 1 + kUltraCheckFrameNumber) % kUltraCheckFrameNumber;
            if (Hands[(int)HandSide.Left].Forearm.localPosition.y >= Ultra.PreviusYs[(int)HandSide.Left, prevois_frame]
                && Hands[(int)HandSide.Right].Forearm.localPosition.y >= Ultra.PreviusYs[(int)HandSide.Right, prevois_frame])
            {
                Ultra.Succeeded = true;
                Debug.Log("Ultra");
            }
        }

        // 値を保存する
        Ultra.PreviusYs[(int)HandSide.Left, Ultra.FrameCounter] = Hands[(int)HandSide.Left].Forearm.localPosition.y;
        Ultra.PreviusYs[(int)HandSide.Right, Ultra.FrameCounter] = Hands[(int)HandSide.Right].Forearm.localPosition.y;
        Ultra.FrameCounter = (Ultra.FrameCounter + 1) % kPunchCheckFrameNumber;
    }
}