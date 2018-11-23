using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEventNavigationState : PlayerNavigationState
{
    private PlayerNavigationState next_ = null;
    private float wait_time_ = 0f;

    public override string Name()
    {
        return "PlayerEventNavigationState";
    }

    public void SetNextState(PlayerNavigationState next)
    {
        next_ = next;
    }

    public void SetWaitTime(float time)
    {
        wait_time_ = time;
    }

    public override void Init(PlayerController player)
    {
        player.NavAgent.isStopped = true;
        player.NavAgent.enabled = false;
    }

    public override void Uninit(PlayerController player)
    {
        wait_time_ = 0f;
        next_ = null;
        player.NavAgent.enabled = true;
    }

    public override void Update(PlayerController player)
    {
        wait_time_ -= Time.deltaTime;
        if(GameManager.Instance.IsLastStage() == false
            && wait_time_ <= 0f
            && player.IsPlayingEvent == false
            && next_ != null)
        {
            player.Change(next_);
        }
    }
}