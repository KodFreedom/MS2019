using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerController : MonoBehaviour
{
    [System.Serializable]
    private struct Finger
    {
        public Transform Main;
        public Transform Bone1;
        public Transform Bone2;
        public Transform Bone3;
    }

    [System.Serializable]
    private struct HandInfo
    {
        public Transform Palm;
        public Transform Forearm;
        public Finger Thumb;
        public Finger Index;
        public Finger Middle;
        public Finger Pinky;
        public Finger Ring;
    }

    [SerializeField] HandInfo LeapMotionLeft;
    [SerializeField] HandInfo LeapMotionRight;
    [SerializeField] HandInfo PlayerModelLeft;
    [SerializeField] HandInfo PlayerModelRight;

    private void LateUpdate()
    {
        UpdateForearm(PlayerModelLeft, LeapMotionLeft, 90f);
        UpdateForearm(PlayerModelRight, LeapMotionRight, -90f);
    }

    private void UpdateForearm(HandInfo model, HandInfo leap_motion, float forearm_offset)
    {
        // palm
        var palm_euler = model.Palm.localEulerAngles;
        palm_euler.z = -leap_motion.Palm.localEulerAngles.x;
        model.Palm.localEulerAngles = palm_euler;

        // forearm
        var forearm_euler = model.Forearm.localEulerAngles;
        forearm_euler.x = -leap_motion.Forearm.localEulerAngles.z + forearm_offset;
        model.Forearm.localEulerAngles = forearm_euler;

        //// Thumb
        //var thumb_euler_1 = model.Thumb.Bone1.localEulerAngles;
        //thumb_euler_1.y = leap_motion.Thumb.Bone1.localEulerAngles.y;
        //model.Thumb.Bone1.localEulerAngles = thumb_euler_1;
    }
}
