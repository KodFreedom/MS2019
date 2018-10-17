using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFieldNavigationState : PlayerNavigationState
{
    private static int kBattleAreaLayer = LayerMask.NameToLayer("BattleArea");

    public override void Init(PlayerController player)
    {
        player.kState = "PlayerFieldNavigationState";
        player.NavAgent.isStopped = false;

        if (player.BattleArea == null)
        {
            player.BattleArea = GameManager.Instance.Data.GetNextBattleArea();
            if (player.BattleArea == null)
            {
                // Stage Clear
                GameManager.Instance.StageClear();

                // Change To event state
                player.Change(player.FieldNavigationState);
            }
        }

        player.NavAgent.SetDestination(player.BattleArea.transform.position);
    }

    public override void Uninit(PlayerController player)
    {
    }

    public override void Update(PlayerController player)
    {
        base.Update(player);
    }

    public override void OnTriggerEnter(PlayerController player, Collider other)
    {
        if (other.gameObject.layer == kBattleAreaLayer)
        {
            player.BattleArea.Register(player);
            player.Change(player.BattleNavigationState);
        }
    }
}