using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThunderMode : PlayerMode
{
    private bool playing_ultra_timeline_ = false;

    public override string Name()
    {
        return "PlayerThunderMode";
    }

    public override float Attack(PlayerController player)
    {
        return player.Parameter.AttackThunder;
    }

    public override void Init(PlayerController player)
    {
        playing_ultra_timeline_ = false;
        // Start Effect
    }

    public override void Uninit(PlayerController player)
    {
        // Stop Effect
    }

    public override void Update(PlayerController player)
    {
        if (playing_ultra_timeline_)
        {
            player.UltraCollider.SetActive(player.EnableUltraCollider);
            if(player.UltraController.state == UnityEngine.Playables.PlayState.Paused)
            {
                playing_ultra_timeline_ = false;
                player.IsPlayingEvent = false;
            }
            return;
        }

        if (player.IsPlayingEvent) return;

        var parameter = player.Parameter;

        // Change mode
        if (player.IsTunderMode == false || parameter.CurrentEnergy <= 0f)
        {
            var input = GameManager.Instance.MyInput;
            input.SetThunderMode(false);
            player.Change(player.NormalMode);
            return;
        }

        // Reduce energy
        parameter.ChangeEnergy(-parameter.ThunderModeCost * Time.deltaTime);

        // Punch
        base.UpdatePunch(player);

        // Ultra
        if (player.Ultra
            && player.MyAnimator.GetFloat("EnableCharge") == 1f
            && parameter.CurrentEnergy >= parameter.UltraCost)
        {
            if(GameManager.Instance.IsLastStage())
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

        var target_group = player.Parameter.UltraTargetGroup;

        if(player.CurrentNavigationState.Name() == player.BattleNavigationState.Name())
        {
            var enemies = player.BattleArea.Enemies;
            int count = 0;
            foreach (var enemy in enemies)
            {
                if (enemy.IsDead) continue;
                enemy.OnExitFight();
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
}