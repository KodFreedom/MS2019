using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class VCamRotationFixer : MonoBehaviour
{
    private CinemachineTransposer vcam_body_ = null;
    private Vector3 direction_ = Vector3.zero;
    private float length_ = 0f;

    private void Start()
    {
        vcam_body_ = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>();
        direction_ = vcam_body_.m_FollowOffset.normalized;
        length_ = vcam_body_.m_FollowOffset.magnitude;
    }

    private void LateUpdate()
    {
        var fixed_direction = GameManager.Instance.Data.Player.transform.rotation * direction_;
        vcam_body_.m_FollowOffset = fixed_direction * length_;
    }
}