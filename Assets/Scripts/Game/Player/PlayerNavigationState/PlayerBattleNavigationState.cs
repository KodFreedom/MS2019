using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBattleNavigationState : PlayerNavigationState
{
    private static int kEnemyLayer = LayerMask.NameToLayer("Enemy");
    private float wait_time_ = 0f;

    public override void Init(PlayerController player)
    {
        player.kState = "PlayerBattleNavigationState";
        FindEnemy(player);
    }

    public override void Uninit(PlayerController player)
    {
    }

    public override void Update(PlayerController player)
    {
        if(wait_time_ > 0f)
        {
            wait_time_ -= Time.deltaTime;
            return;
        }

        if (player.TargetEnemy == null)
        {
            FindEnemy(player);
        }

        if (player.NavAgent.isStopped == true)
        {
            if(player.TargetEnemy.IsDead)
            {
                player.TargetEnemy = null;
                wait_time_ = 1f;
            }
        }
    }

    public override void OnTriggerEnter(PlayerController player, Collider other)
    {
        if (other.gameObject.layer == kEnemyLayer)
        {
            if (other.GetComponent<EnemyController>() == player.TargetEnemy)
            {
                // 目標にたどり着いた
                player.NavAgent.isStopped = true;
            }
        }
    }

    private void FindEnemy(PlayerController player)
    {
        player.TargetEnemy = player.BattleArea.GetNearestEnemy(player.transform.position);
        if (player.TargetEnemy == null)
        {
            // battle area clear
            player.BattleArea = null;

            // change to move state
            player.Change(player.FieldNavigationState);
        }
        else
        {
            // enemyに向かって移動する
            player.NavAgent.SetDestination(player.TargetEnemy.transform.position);
            player.NavAgent.isStopped = false;
        }
    }
}