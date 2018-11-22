using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNormalMode : PlayerMode
{
    private bool playing_ultra_timeline_ = false;
    private float counter_effect_counter_ = 0f;

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
    }

    public override void Uninit(PlayerController player)
    {
    }

    public override void OnHitted(PlayerController player)
    {
        player.MyAnimator.SetBool("Hitted", true);
    }

    public override void Update(PlayerController player)
    {
        player.UltraCollider.SetActive(player.EnableUltraCollider);
        player.PunchCollider.SetActive(player.MyAnimator.GetFloat("EnablePunchCollider") == 1f);

        if (playing_ultra_timeline_)
        {
            if (player.UltraController.state == UnityEngine.Playables.PlayState.Paused)
            {
                playing_ultra_timeline_ = false;
                player.IsPlayingEvent = false;
            }
            return;
        }

        if (player.IsPlayingEvent)
        {
            player.Parameter.kScriptableTimeScale = 1f;
            return;
        }

        // Punch&Counter
        UpdatePunchAndCounter(player);

        // Ultra
        UpdateUltra(player);
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

    private void UpdatePunchAndCounter(PlayerController player)
    {
        var parameter = player.Parameter;
        bool left_punch = false;
        bool right_punch = false;
        bool left_counter_punch = false;
        bool right_counter_punch = false;

        if (counter_effect_counter_ > 0f)
        {
            counter_effect_counter_ -= Time.unscaledDeltaTime;
            if(counter_effect_counter_ <= 0f)
            {
                counter_effect_counter_ = 0f;
                parameter.CounterCheckDelayCounter = -1f;
            }
            var rate = 1f - counter_effect_counter_ / parameter.CounterEffectTime;
            parameter.kScriptableTimeScale = parameter.CounterTimeScale.Evaluate(rate);
        }
        else if(player.LeftPunch || player.RightPunch)
        {
            if (parameter.CounterCheckDelayCounter > 0f
                && player.MyAnimator.GetBool("EnableCounter"))
            {// Counter Punch
                parameter.CounterCheckDelayCounter = float.MaxValue;
                foreach (var enemy in parameter.CounterTargets)
                {
                    if (enemy == null) continue;
                    parameter.ChangeEnergy(player.Parameter.CounterEnergy);
                    Debug.Log("Counter Successed : " + enemy.gameObject.name);
                }

                parameter.ClearCounterTargets();
                counter_effect_counter_ = parameter.CounterEffectTime;
                left_counter_punch = player.LeftPunch;
                right_counter_punch = player.RightPunch;
            }
            else
            {// Normal Punch
                left_punch = player.LeftPunch;
                right_punch = player.RightPunch;
            }
        }

        Debug.Log(left_counter_punch.ToString() + right_counter_punch.ToString()
            + left_punch.ToString() + right_punch.ToString());
        player.MyAnimator.SetBool("LeftCounterPunch", left_counter_punch);
        player.MyAnimator.SetBool("RightCounterPunch", right_counter_punch);
        player.MyAnimator.SetBool("LeftPunch", left_punch);
        player.MyAnimator.SetBool("RightPunch", right_punch);
    }
}