using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThunderMode : PlayerMode
{
    public override float Attack(PlayerController player)
    {
        return player.Parameter.AttackThunder;
    }

    public override void Init(PlayerController player)
    {
        player.kMode = "PlayerThunderMode";

        // Start Effect
    }

    public override void Uninit(PlayerController player)
    {
        // Stop Effect
    }

    public override void Update(PlayerController player)
    {
        if (player.IsPlayingEvent)
        {
            player.UltraCollider.SetActive(player.EnableUltraCollider);
            return;
        }

        // Reduce energy
        var parameter = player.Parameter;
        parameter.ChangeEnergy(-parameter.ThunderModeCost * Time.deltaTime);

        // Punch
        player.PunchCollider.SetActive(player.MyAnimator.GetFloat("EnablePunchCollider") == 1f);

        // Ultra
        if (player.Ultra
            && player.MyAnimator.GetFloat("EnableCharge") == 1f
            && parameter.CurrentEnergy >= parameter.UltraCost)
        {
            StartUltra(player);
        }

        // Change mode
        if(player.ModeChange || parameter.CurrentEnergy <= 0f)
        {
            player.Change(player.NormalMode);
        }
    }

    private void StartUltra(PlayerController player)
    {
        var target_group = player.Parameter.UltraTargetGroup;
        var enemies = player.BattleArea.Enemies;
        int target_number = enemies.Count + 1;
        float weight = 1f / target_number;
        target_group.m_Targets = new Cinemachine.CinemachineTargetGroup.Target[enemies.Count + 1];

        // player
        target_group.m_Targets[0].target = player.transform;
        target_group.m_Targets[0].weight = weight;

        // enemy
        int count = 1;
        foreach(var enemy in enemies)
        {
            enemy.OnExitFight();
            target_group.m_Targets[count].target = enemy.transform;
            target_group.m_Targets[count].weight = weight;
            ++count;
        }

        player.PunchCollider.SetActive(false);
        player.IsPlayingEvent = true;
        player.UltraController.Play();
    }
}