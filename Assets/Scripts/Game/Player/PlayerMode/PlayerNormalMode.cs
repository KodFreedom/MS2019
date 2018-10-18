﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNormalMode : PlayerMode
{
    public override float Attack(PlayerController player)
    {
        return player.Parameter.AttackNormal;
    }

    public override void Init(PlayerController player)
    {
        player.kMode = "PlayerNormalMode";
    }

    public override void Uninit(PlayerController player)
    {

    }

    public override void Update(PlayerController player)
    {
        player.PunchCollider.SetActive(player.MyAnimator.GetFloat("EnablePunchCollider") == 1f);

        if (player.ModeChange == true)
        {
            player.Change(player.ThunderMode);
        }
    }
}