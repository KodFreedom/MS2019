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
        public ChargingManager chargingEffect;
    }

    [SerializeField] float kUltraCost = 50f;
    [SerializeField] float kChargeSpeed = 1f;
    [SerializeField] PunchInfo kPunchInfo = new PunchInfo();
    [SerializeField] UltraInfo kUltraInfo = new UltraInfo();
    [SerializeField] KnockbackInfo kKnockbackInfo = new KnockbackInfo();
    [SerializeField] CinemachineTargetGroup kUltraTargetGroup;
    [SerializeField] HandEffect kLeftHandEffects;
    [SerializeField] HandEffect kRightHandEffects;
    [SerializeField] float kCounterEnergy = 50f;
    [SerializeField] float kCounterCheckDelay = 0.2f;
    [SerializeField] float kCounterEffectTime = 0.5f;
    [SerializeField] AnimationCurve kCounterTimeScale;

    public float kTimeScale = 1f;
    public float kScriptableTimeScale = 1f;
    public float MaxEnergy { get; private set; }
    public float CurrentEnergy { get; private set; }
    public float ChargedEnergy { get; private set; }
    public float UltraCost { get { return kUltraCost; } }
    public float CounterEnergy { get { return kCounterEnergy; } }
    public float CounterCheckDelay { get { return kCounterCheckDelay; } }
    public float CounterEffectTime { get { return kCounterEffectTime; } }
    public float CounterCheckDelayCounter = 0f;
    public float ChargeSpeed { get { return kChargeSpeed; } }
    public float AttackNormal { get { return kPunchInfo.attackNormal; } }
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
    public List<GameObject> CounterTargets { get; private set; }
    public AnimationCurve CounterTimeScale { get { return kCounterTimeScale; } }

    public void ChangeEnergy(float amount)
    {
        CurrentEnergy = Mathf.Clamp(CurrentEnergy + amount, 0f, MaxEnergy);
        ChargedEnergy += Mathf.Max(0f, amount);
    }

    public void Tick(float delta_time)
    {
        Timer += delta_time;
    }

    public void Register(GameObject counter_target)
    {
        if (counter_target == null) return;
        if (CounterTargets.Contains(counter_target)) return;
        CounterTargets.Add(counter_target);
    }

    public void ClearCounterTargets()
    {
        CounterTargets.Clear();
    }

    private void Start()
    {
        MaxEnergy = 100f;
        CurrentEnergy = 0f;
        Timer = 0f;
        CounterTargets = new List<GameObject>();
    }
}
