using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParameter : MonoBehaviour
{
    [SerializeField] float kThunderModeCost = 1f;
    [SerializeField] float kUltraCost = 50f;
    [SerializeField] float kChargeSpeed = 1f;
    [SerializeField] float kAttackNormal = 1f;
    [SerializeField] float kAttackThunder = 1.5f;

    public float MaxEnergy { get; private set; }
    public float CurrentEnergy { get; private set; }
    public float ThunderModeCost { get { return kThunderModeCost; } }
    public float UltraCost { get { return kUltraCost; } }
    public float ChargeSpeed { get { return kChargeSpeed; } }
    public float AttackNormal { get { return kAttackNormal; } }
    public float AttackThunder { get { return kAttackThunder; } }
    public float Timer { get; private set; }

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
