using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBattleNavigationState : PlayerNavigationState
{
    private static int kEnemyLayer;
    private float wait_time_ = 0f;
    private float knockback_speed_ = 0f;
    private float knockback_acc_ = 0f;
    private float knockback_freeze_time_ = 0f;

    public override string Name()
    {
        return "PlayerBattleNavigationState";
    }

    public override void Init(PlayerController player)
    {
        kEnemyLayer = LayerMask.NameToLayer("Enemy");
        player.NavAgent.angularSpeed = 360f;
    }

    public override void Uninit(PlayerController player)
    {
    }

    public override void Update(PlayerController player)
    {
        base.Update(player);
        if (player.IsPlayingEvent) return;

        if (knockback_acc_ != 0f)
        {
            Knockback(player);
        }
        else
        {
            Navgation(player);
        }
    }

    public override void OnTriggerEnter(PlayerController player, Collider other)
    {
        if (other.gameObject.layer == kEnemyLayer)
        {
            if (other.gameObject == player.TargetEnemy.gameObject)
            {
                player.NavAgent.isStopped = true;
                player.TargetEnemy.OnBeginFight();
            }
            else if (other.gameObject.name.Equals("PunchCollider"))
            {
                Hitted(player);
            }
        }
    }

    public override void OnTriggerExit(PlayerController player, Collider other)
    {
        if (other.gameObject.layer == kEnemyLayer)
        {
            if (other.GetComponent<EnemyController>() == player.TargetEnemy)
            {
                player.TargetEnemy.OnExitFight();
            }
        }
    }

    private void FindEnemy(PlayerController player)
    {
        player.TargetEnemy = player.BattleArea.GetNearestEnemy(player.transform.position);
        if (player.TargetEnemy == null)
        {
            player.BattleArea.OnBattleAreaClear();

            // battle area clear
            player.BattleArea = null;

            // change to event state
            player.EventNavigationState.SetNextState(player.FieldNavigationState);
            player.Change(player.EventNavigationState);
        }
        else
        {
            // enemyに向かって移動する
            player.NavAgent.SetDestination(player.TargetEnemy.transform.position);
            player.NavAgent.isStopped = false;
        }
    }

    private void Navgation(PlayerController player)
    {
        if (wait_time_ > 0f)
        {
            wait_time_ -= Time.deltaTime;
            return;
        }

        if (player.TargetEnemy == null)
        {
            FindEnemy(player);
        }
        else
        {
            if (player.TargetEnemy.IsDead)
            {
                player.TargetEnemy = null;
                wait_time_ = 1f;
            }
        }
    }

    private void Hitted(PlayerController player)
    {
        // v0t = 2vpt0
        float player_speed = player.NavAgent.speed;
        knockback_speed_ = player.Parameter.KnockbackSpeed;
        float knockback_time = 2 * player_speed * player.Parameter.KnockbackReturnTime / knockback_speed_;
        knockback_acc_ = -knockback_speed_ / knockback_time;

        player.OnDamaged();
    }

    private void Knockback(PlayerController player)
    {
        if(knockback_freeze_time_ > 0f)
        {
            knockback_freeze_time_ -= Time.deltaTime;
            if(knockback_freeze_time_ <= 0f)
            {
                knockback_speed_ = 0f;
                knockback_acc_ = 0f;
                player.NavAgent.isStopped = false;
            }
            return;
        }

        player.transform.position += -player.transform.forward * knockback_speed_ * Time.deltaTime;
        knockback_speed_ += knockback_acc_ * Time.deltaTime;

        if(knockback_speed_ < 0f)
        {
            knockback_freeze_time_ = player.Parameter.KnockbackFreezeTime;
        }
    }
}