using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerNavigationState
{
    public abstract string Name();

    public abstract void Init(PlayerController player);

    public abstract void Uninit(PlayerController player);

    public abstract void Update(PlayerController player);

    public virtual void OnTriggerEnter(PlayerController player, Collider other) { }

    public virtual void OnTriggerExit(PlayerController player, Collider other) { }

    protected bool CheckArrive(PlayerController player)
    {
        var agent = player.NavAgent;

        // Check if we've reached the destination
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
