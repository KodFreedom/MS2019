using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBattleNavigationState : PlayerNavigationState
{
    private float knockback_speed_ = 0f;
    private float knockback_acc_ = 0f;
    private float knockback_freeze_time_ = 0f;

    public override string Name()
    {
        return "PlayerBattleNavigationState";
    }

    public override void Init(PlayerController player)
    {
        player.NavAgent.isStopped = true;
        player.Parameter.CounterCheckDelayCounter = -1f;
    }

    public override void Uninit(PlayerController player)
    {
    }

    public override void Update(PlayerController player)
    {
        if (player.IsPlayingEvent) return;

        CheckHitted(player);

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
        if (other.gameObject.CompareTag("EnemyAttackCollider"))
        {
            Debug.Log("OnEnemyPunchEnter");
            var parameter = player.Parameter;
            parameter.Register(other.GetComponentInParent<EnemyController>());
            if(parameter.CounterCheckDelayCounter < 0f)
            {
                parameter.CounterCheckDelayCounter = parameter.CounterCheckDelay;
            }
        }
    }

    private void Navgation(PlayerController player)
    {
        if (player.BattleArea.Paused == false)
        {
            FindEnemy(player);
        }
        else
        {
            if (CheckArrive(player) == true)
            {
                player.NavAgent.isStopped = true;
                player.BattleArea.OnBattleResume();
            }
        }
    }

    private void FindEnemy(PlayerController player)
    {
        if(player.TargetEnemy)
        {
            if(player.TargetEnemy.IsDead)
            {
                player.TargetEnemy = null;
            }
        }
        else
        {
            player.TargetEnemy = player.BattleArea.GetNearestEnemy(player.transform.position);
            if (player.TargetEnemy)
            {
                player.TargetEnemy.OnBeginFight();
            }
            else
            {
                // battle area clear
                player.BattleArea.OnBattleAreaClear();
                player.BattleArea = null;

                // change to event state
                player.EventNavigationState.SetNextState(player.FieldNavigationState);
                player.Change(player.EventNavigationState);
            }
        }
    }

    private void CheckHitted(PlayerController player)
    {
        if(player.Parameter.CounterCheckDelayCounter > 0f)
        {
            player.Parameter.CounterCheckDelayCounter -= Time.deltaTime;
            
            if(player.Parameter.CounterCheckDelayCounter <= 0f)
            {
                // v0t = 2vpt0
                float player_speed = player.NavAgent.speed;
                knockback_speed_ = player.Parameter.KnockbackSpeed;
                float knockback_time = 2 * player_speed * player.Parameter.KnockbackReturnTime / knockback_speed_;
                knockback_acc_ = -knockback_speed_ / knockback_time;

                player.OnDamaged();
                player.Parameter.ClearCounterTargets();
                player.BattleArea.OnBattlePause();
            }
        }
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
                player.NavAgent.SetDestination(player.BattleArea.transform.position);
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