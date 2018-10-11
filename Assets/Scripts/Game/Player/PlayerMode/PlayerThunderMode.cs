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
        base.Update(player);

        var parameter = player.Parameter;
        parameter.ChangeEnergy(-parameter.ThunderModeCost * Time.deltaTime);

        if(player.Ultra)
        {
            // Ultra
        }

        if(player.ModeChange == true || parameter.CurrentEnergy <= 0f)
        {
            player.Change(player.NormalMode);
        }
    }
}