﻿using System.Collections;
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
        knockback_speed_ = 0f;
        knockback_acc_ = 0f;
        knockback_freeze_time_ = 0f;
        player.NavAgent.isStopped = true;
        player.TargetEnemy = null;
        player.Parameter.ClearCounterTargets();
        player.Parameter.CounterCheckDelayCounter = -1f;
    }

    public override void Uninit(PlayerController player)
    {
    }

    public override void Update(PlayerController player)
    {
        if (player.IsPlayingEvent)
        {
            player.Parameter.ClearCounterTargets();
            player.Parameter.CounterCheckDelayCounter = -1f;
            if (knockback_acc_ != 0f)
            {
                knockback_freeze_time_ = Time.deltaTime;
            }
            return;
        }

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
        if (knockback_acc_ != 0f) return;

        if (other.gameObject.CompareTag("EnemyAttackCollider"))
        {
            Debug.Log("OnEnemyAttackEnter");
            var parameter = player.Parameter;
            parameter.Register(other.gameObject);
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
                SoundManager.Instance.StopLoopSe("Game_walk000", 0f);
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
                player.EventNavigationState.SetWaitTime(0.5f);
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
            {// Hitted
                // v0t = 2vpt0
                float player_speed = player.NavAgent.speed;
                knockback_speed_ = player.Parameter.KnockbackSpeed;
                float knockback_time = 2 * player_speed * player.Parameter.KnockbackReturnTime / knockback_speed_;
                knockback_acc_ = -knockback_speed_ / knockback_time;

                player.OnDamaged();
                player.BattleArea.OnBattlePause();

                var parameter = player.Parameter;
                foreach (var enemy in parameter.CounterTargets)
                {
                    if (enemy == null) continue;
                    if (enemy.name.Contains("Can"))
                    {
                        enemy.GetComponent<ThrowItemController>().OnClear();
                    }
                }
                parameter.ClearCounterTargets();
                parameter.CounterCheckDelayCounter = -1f;

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
                SoundManager.Instance.PlaySe("Game_walk000", true);
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