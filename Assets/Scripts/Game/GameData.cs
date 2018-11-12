using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    private List<BattleAreaController> kBattleAreas = new List<BattleAreaController>();
    public PlayerController Player { get; private set; }
    public ResultData Result { get; private set; }

    public BattleAreaController GetNextBattleArea()
    {
        if (kBattleAreas.Count == 0) return null;
        var result = kBattleAreas[0];
        kBattleAreas.Remove(result);
        return result;
    }

    public void Register(List<BattleAreaController> battle_areas)
    {
        kBattleAreas.Clear();
        kBattleAreas = battle_areas;
    }

    public void Register(PlayerController player)
    {
        Player = player;
    }

    public void Register(ResultData result)
    {
        Result = result;
    }
}