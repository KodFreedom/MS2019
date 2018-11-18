using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerParameter : MonoBehaviour
{
    [System.Serializable]
    private struct KnockbackInfo
    {
        public float speed;
        public float returnTime;
        public float freezeTime;
        public float cameraShakeRange;
        public float cameraShakeTime;
    }

    [System.Serializable]
    private struct PunchInfo
    {
        public float attackNormal;
        public float attackThunder;
        public float cameraShakeRange;
        public float cameraShakeTime;
    }

    [System.Serializable]
    private struct UltraInfo
    {
        public float cameraShakeRange;
        public float cameraShakeTime;
    }

    [System.Serializable]
    public struct HandEffect
    {
        public Transform hand;
        public ChargedParticle chargeEffect;
        public ParticleSystem thunderModeEffect;
    }

    [SerializeField] float kThunderModeCost = 1f;
    [SerializeField] float kUltraCost = 50f;
    [SerializeField] float kChargeSpeed = 1f;
    [SerializeField] PunchInfo kPunchInfo = new PunchInfo();
    [SerializeField] UltraInfo kUltraInfo = new UltraInfo();
    [SerializeField] KnockbackInfo kKnockbackInfo = new KnockbackInfo();
    [SerializeField] CinemachineTargetGroup kUltraTargetGroup;
    [SerializeField] HandEffect kLeftHandEffects;
    [SerializeField] HandEffect kRightHandEffects;

    public float kTimeScale = 1f;

    public float MaxEnergy { get; private set; }
    public float CurrentEnergy { get; private set; }
    public float ThunderModeCost { get { return kThunderModeCost; } }
    public float UltraCost { get { return kUltraCost; } }
    public float ChargeSpeed { get { return kChargeSpeed; } }
    public float AttackNormal { get { return kPunchInfo.attackNormal; } }
    public float AttackThunder { get { return kPunchInfo.attackThunder; } }
    public float PunchCameraShakeRange { get { return kPunchInfo.cameraShakeRange; } }
    public float PunchCameraShakeTime { get { return kPunchInfo.cameraShakeTime; } }
    public float UltraCameraShakeRange { get { return kUltraInfo.cameraShakeRange; } }
    public float UltraCameraShakeTime { get { return kUltraInfo.cameraShakeTime; } }
    public float Timer { get; private set; }
    public float KnockbackSpeed { get { return kKnockbackInfo.speed; } }
    public float KnockbackReturnTime { get { return kKnockbackInfo.returnTime; } }
    public float KnockbackFreezeTime { get { return kKnockbackInfo.freezeTime; } }
    public float KnockbackCameraShakeRange { get { return kKnockbackInfo.cameraShakeRange; } }
    public float KnockbackCameraShakeTime { get { return kKnockbackInfo.cameraShakeTime; } }
    public CinemachineTargetGroup UltraTargetGroup { get { return kUltraTargetGroup; } }
    public HandEffect LeftHandEffects { get { return kLeftHandEffects; } }
    public HandEffect RightHandEffects { get { return kRightHandEffects; } }

    public void ChangeEnergy(float amount)
    {
        CurrentEnergy = Mathf.Clamp(CurrentEnergy + amount, 0f, MaxEnergy);
    }

    public void Tick(float delta_time)
    {
        Timer += delta_time;
    }

    private void Start()
    {
        MaxEnergy = 100f;
        CurrentEnergy = 0f;
        Timer = 0f;
    }
}
