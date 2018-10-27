using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNormalMode : PlayerMode
{
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
    }

    public override void Uninit(PlayerController player)
    {

    }

    public override void Update(PlayerController player)
    {
        if (player.IsPlayingEvent) return;

        if (player.IsTunderMode == true)
        {
            player.Change(player.ThunderMode);
            return;
        }

        base.UpdatePunch(player);
    }
}