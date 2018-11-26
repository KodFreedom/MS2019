using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFieldNavigationState : PlayerNavigationState
{
    private float speed_ = 3.5f;
    private float rotate_speed_ = 24f;

    public override string Name()
    {
        return "PlayerFieldNavigationState";
    }

    public override void Init(PlayerController player)
    {
        player.NavAgent.isStopped = false;
        player.NavAgent.angularSpeed = 24f;
        SoundManager.Instance.PlaySe("Game_walk000", true);
    }

    public override void Uninit(PlayerController player)
    {
        SoundManager.Instance.StopLoopSe("Game_walk000", 0f);
    }

    public override void Update(PlayerController player)
    {
        if (player.IsPlayingEvent)
        {
            player.NavAgent.speed = 0f;
            player.NavAgent.angularSpeed = 0f;
            SoundManager.Instance.StopLoopSe("Game_walk000", 0f);
        }
        else
        {
            player.NavAgent.speed = speed_;
            player.NavAgent.angularSpeed = rotate_speed_;
            SoundManager.Instance.PlaySe("Game_walk000", true);
        }

        if(player.BattleArea)
        {
            if(CheckArrive(player))
            {
                player.BattleArea.OnBattleStart(player);
                player.EventNavigationState.SetNextState(player.BattleNavigationState);
                player.Change(player.EventNavigationState);
            }
        }
        else
        {
            FindBattleArea(player);
        }  
    }

    private void FindBattleArea(PlayerController player)
    {
        player.BattleArea = GameManager.Instance.Data.GetNextBattleArea();
        if (player.BattleArea == null)
        {
            // Stage Clear
            GameManager.Instance.StageClear();

            // Change To event state
            player.EventNavigationState.SetNextState(player.FieldNavigationState);
            player.Change(player.EventNavigationState);
            return;
        }

        player.NavAgent.SetDestination(player.BattleArea.transform.position);
    }
}