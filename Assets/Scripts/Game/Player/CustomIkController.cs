using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CustomIkController : MonoBehaviour
{
    //public Transform kLookPosition = null;
    public Transform kLeftHand = null;
    public Transform kRightHand = null;

    [SerializeField] Vector2 kRotateRangeL = Vector2.zero;
    [SerializeField] Vector2 kRotateRangeR = Vector2.zero;
    private float weight_ = 1f;
    private Vector3 left_hand_euler_ = Vector3.zero;
    private Vector3 right_hand_euler_ = Vector3.zero;

    public void SetActive(bool active)
    {
        float sign = active == true ? 1f : -1f;
        weight_ = Mathf.Clamp(weight_ + sign * Time.deltaTime * 10f, 0f, 1f);
        left_hand_euler_ *= weight_;
        right_hand_euler_ *= weight_;
    }

    public void RotateLeft(float amount)
    {
        left_hand_euler_.x = Mathf.Clamp(left_hand_euler_.x + amount, kRotateRangeL.x, kRotateRangeL.y);
    }

    public void RotateRight(float amount)
    {
        right_hand_euler_.x = Mathf.Clamp(right_hand_euler_.x + amount, kRotateRangeR.x, kRotateRangeR.y);
    }

    public void SetRotationLeft(float amount)
    {
        left_hand_euler_.x = amount;
    }

    public void SetRotationRight(float amount)
    {
        right_hand_euler_.x = amount;
    }

    // Use this for initialization
    private void Start ()
    {
        SetActive(true);
	}

    private void Update()
    {
    }

    private void LateUpdate()
    {
        UpdateLookIK();
        UpdateHandIK();
    }

    private void UpdateLookIK()
    {
    }

    private void UpdateHandIK()
    {
        if (kLeftHand)
        {
            var euler = kLeftHand.localEulerAngles;
            euler.x = Mathf.Lerp(euler.x, left_hand_euler_.x, weight_);
            kLeftHand.localEulerAngles = euler;
        }

        if (kRightHand)
        {
            var euler = kRightHand.localEulerAngles;
            euler.x = Mathf.Lerp(euler.x, right_hand_euler_.x, weight_);
            kRightHand.localEulerAngles = euler;
        }
    }
}
