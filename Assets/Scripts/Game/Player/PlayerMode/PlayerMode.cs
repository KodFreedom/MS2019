using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerMode
{
    public abstract float Attack(PlayerController player);

    public abstract void Init(PlayerController player);

    public abstract void Uninit(PlayerController player);

    public virtual void Update(PlayerController player) { }

    public virtual void OnTriggerEnter(PlayerController player, Collider other) { }

    public virtual void OnTriggerExit(PlayerController player, Collider other) { }
}