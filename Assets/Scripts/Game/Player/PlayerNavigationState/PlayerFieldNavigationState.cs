using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFieldNavigationState : PlayerNavigationState
{
    private static int kBattleAreaLayer = LayerMask.NameToLayer("BattleArea");

    public override string Name()
    {
        return "PlayerFieldNavigationState";
    }

    public override void Init(PlayerController player)
    {
        player.NavAgent.isStopped = false;
        player.NavAgent.angularSpeed = 18f;
    }

    public override void Uninit(PlayerController player)
    {
    }

    public override void Update(PlayerController player)
    {
        if (player.IsPlayingEvent) return;
        FindBattleArea(player);
    }

    public override void OnTriggerEnter(PlayerController player, Collider other)
    {
        if (other.gameObject.layer == kBattleAreaLayer)
        {
            player.BattleArea.OnBattleAreaEnter(player);
            player.EventNavigationState.SetNextState(player.BattleNavigationState);
            player.Change(player.EventNavigationState);
        }
    }

    private void FindBattleArea(PlayerController player)
    {
        if (player.BattleArea == null)
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
        }

        player.NavAgent.SetDestination(player.BattleArea.transform.position);
    }
}