using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNormalMode : PlayerMode
{
    private bool playing_ultra_timeline_ = false;
    private float counter_effect_counter_ = 0f;
    private int left_layer_index_ = 0;
    private int right_layer_index_ = 0;
    private int current_layer_index_ = 0;

    public override string Name()
    {
        return "PlayerNormalMode";
    }

    public override float Attack(PlayerController player)
    {
        return player.Parameter.AttackNormal;
    }

    public override void Init(PlayerController player)
    {
        playing_ultra_timeline_ = false;
        left_layer_index_ = player.MyAnimator.GetLayerIndex("Left Layer");
        right_layer_index_ = player.MyAnimator.GetLayerIndex("Right Layer");
    }

    public override void Uninit(PlayerController player)
    {

    }

    public override void Update(PlayerController player)
    {
        player.UltraCollider.SetActive(player.EnableUltraCollider);

        if (playing_ultra_timeline_)
        {
            if (player.UltraController.state == UnityEngine.Playables.PlayState.Paused)
            {
                playing_ultra_timeline_ = false;
                player.IsPlayingEvent = false;
            }
            return;
        }

        if (player.IsPlayingEvent) return;

        // Punch
        UpdatePunch(player);

        // Counter
        UpdateCounter(player);

        // Ultra
        UpdateUltra(player);
    }

    private void UpdatePunch(PlayerController player)
    {
        player.MyAnimator.SetBool("LeftPunch", player.LeftPunch);
        player.MyAnimator.SetBool("RightPunch", player.RightPunch);

        bool enable_punch_collider = player.MyAnimator.GetFloat("EnablePunchCollider") == 1f;
        player.PunchCollider.SetActive(enable_punch_collider);
        player.Parameter.SetEnableCounter(enable_punch_collider);
    }

    private void UpdateUltra(PlayerController player)
    {
        var parameter = player.Parameter;
        if (player.Ultra
            && player.MyAnimator.GetFloat("EnableCharge") == 1f
            && parameter.CurrentEnergy >= parameter.UltraCost)
        {
            if (GameManager.Instance.IsLastStage())
            {
                GameManager.Instance.StageClear();
            }
            else
            {
                StartUltra(player);
            }
        }
    }

    private void StartUltra(PlayerController player)
    {
        playing_ultra_timeline_ = true;
        player.IsPlayingEvent = true;
        player.Parameter.ChangeEnergy(-player.Parameter.UltraCost);

        // Set target
        var target_group = player.Parameter.UltraTargetGroup;

        if (player.CurrentNavigationState.Name() == player.BattleNavigationState.Name())
        {
            player.BattleArea.OnBattlePause();
            var enemies = player.BattleArea.Enemies;
            int count = 0;
            foreach (var enemy in enemies)
            {
                if (enemy.IsDead) continue;
                ++count;
            }

            int target_number = count + 1;
            float weight = 1f / target_number;
            target_group.m_Targets = new Cinemachine.CinemachineTargetGroup.Target[enemies.Count + 1];

            // player
            target_group.m_Targets[0].target = player.transform;
            target_group.m_Targets[0].weight = weight;

            // enemy
            count = 1;
            foreach (var enemy in enemies)
            {
                if (enemy.IsDead) continue;
                target_group.m_Targets[count].target = enemy.transform;
                target_group.m_Targets[count].weight = weight;
                ++count;
            }
        }
        else
        {
            target_group.m_Targets[0].target = player.transform;
            target_group.m_Targets[0].weight = 1f;
        }

        player.PunchCollider.SetActive(false);
        player.UltraController.Play();
    }

    private void UpdateCounter(PlayerController player)
    {
        var parameter = player.Parameter;

        if (counter_effect_counter_ > 0f)
        {
            counter_effect_counter_ -= Time.unscaledDeltaTime;
            if(counter_effect_counter_ <= 0f)
            {
                counter_effect_counter_ = 0f;
                player.MyAnimator.SetLayerWeight(current_layer_index_, 0f);
                parameter.CounterCheckDelayCounter = -1f;
            }
            var rate = 1f - counter_effect_counter_ / parameter.CounterEffectTime;
            parameter.kScriptableTimeScale = parameter.CounterTimeScale.Evaluate(rate);

            // TODO: 0-0.2: 0-1 // 0.8-1: 1-0
            //var weight = rate <= 0.1f ? rate / 0.1f : rate >= 0.9f ? (1f - rate) / 0.1f : 1f;
            //Debug.Log(current_layer_index_ + " : " + weight);
            Debug.Log("CounterEffect");
            return;
        }

        if(parameter.EnableCounter
            && parameter.CounterCheckDelayCounter > 0f)
        {
            parameter.CounterCheckDelayCounter = float.MaxValue;
            foreach(var enemy in parameter.CounterTargets)
            {
                if (enemy == null) continue;
                parameter.ChangeEnergy(player.Parameter.CounterEnergy);
                Debug.Log("Counter Successed : " + enemy.gameObject.name);
            }

            parameter.ClearCounterTargets();
            counter_effect_counter_ = parameter.CounterEffectTime;
            current_layer_index_ = player.MyAnimator.GetCurrentAnimatorStateInfo(0).IsName("LeftPunch") ? right_layer_index_ : left_layer_index_;
            player.MyAnimator.SetLayerWeight(current_layer_index_, 1f);
            //player.MyAnimator.Play("Counter", current_layer_index_);
        }
    }
}