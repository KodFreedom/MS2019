using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerNavigationState
{
    private int previous_enable_navgation_ = -1;

    public abstract string Name();

    public abstract void Init(PlayerController player);

    public abstract void Uninit(PlayerController player);

    public virtual void Update(PlayerController player)
    {
        if(player.IsPlayingEvent)
        {
            if(previous_enable_navgation_ == -1)
            {
                previous_enable_navgation_ = player.NavAgent.isStopped == true ? 0 : 1;
                player.NavAgent.isStopped = true;
            }
        }
        else
        {
            if (previous_enable_navgation_ != -1)
            {
                player.NavAgent.isStopped = previous_enable_navgation_ == 1 ? false : true;
                previous_enable_navgation_ = -1;
            }
        }
    }

    public virtual void OnTriggerEnter(PlayerController player, Collider other) { }

    public virtual void OnTriggerExit(PlayerController player, Collider other) { }
}
