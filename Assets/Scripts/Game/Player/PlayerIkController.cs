using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIkController : MonoBehaviour
{
    public Transform kLookPosition = null;
    public Transform kLeftHandIkHandler = null;
    public Transform kRightHandIkHandler = null;

    private Animator animator_ = null;
    private float weight_ = 1f;

    public void SetActive(bool active)
    {
        float sign = active == true ? 1f : -1f;
        weight_ = Mathf.Clamp(weight_ + sign * Time.deltaTime * 10f, 0f, 1f);
    }

	// Use this for initialization
	private void Start ()
    {
        animator_ = GetComponent<Animator>();
        SetActive(true);
	}

    private void OnAnimatorIK(int layerIndex)
    {
        UpdateLookIK();
        UpdateHandIK();
    }

    private void UpdateLookIK()
    {
        animator_.SetLookAtWeight(weight_);
        animator_.SetLookAtPosition(kLookPosition.position);
    }

    private void UpdateHandIK()
    {
        animator_.SetIKPositionWeight(AvatarIKGoal.LeftHand, weight_);
        animator_.SetIKRotationWeight(AvatarIKGoal.LeftHand, weight_);
        animator_.SetIKPositionWeight(AvatarIKGoal.RightHand, weight_);
        animator_.SetIKRotationWeight(AvatarIKGoal.RightHand, weight_);

        if (kLeftHandIkHandler)
        {
            animator_.SetIKPosition(AvatarIKGoal.LeftHand, kLeftHandIkHandler.position);
            animator_.SetIKRotation(AvatarIKGoal.LeftHand, kLeftHandIkHandler.rotation);
        }

        if (kRightHandIkHandler)
        {
            animator_.SetIKPosition(AvatarIKGoal.RightHand, kRightHandIkHandler.position);
            animator_.SetIKRotation(AvatarIKGoal.RightHand, kRightHandIkHandler.rotation);
        }
    }
}
