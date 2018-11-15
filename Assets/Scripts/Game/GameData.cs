using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    private List<BattleAreaController> battle_areas_ = new List<BattleAreaController>();
    public PlayerController Player { get; private set; }
    public ResultController Result { get; private set; }
    public InputManager MyInput { get; private set; }
    public CinemachineManager Cinemachines { get; private set; }
    public EventFadeController EventFadeIn { get; private set; }
    public EventFadeController EventFadeOut { get; private set; }
    public Light SunLight { get; private set; }
    public StageLoader MyStageLoader { get; private set; }

    public bool GetReady()
    {
        return Player != null
            && Result != null
            && MyInput != null
            && Cinemachines != null
            && EventFadeIn != null
            && EventFadeOut != null
            && SunLight != null
            && MyStageLoader != null;
    }

    public BattleAreaController GetNextBattleArea()
    {
        if (battle_areas_.Count == 0) return null;
        var result = battle_areas_[0];
        battle_areas_.Remove(result);
        return result;
    }

    public void Register(List<BattleAreaController> battle_areas)
    {
        battle_areas_.Clear();
        battle_areas_ = battle_areas;
    }

    public void Register(PlayerController player)
    {
        Player = player;
    }

    public void Register(ResultController result)
    {
        Result = result;
    }

    public void Register(EventFadeController event_fade, EventFadeController.FadeState state)
    {
        if(state == EventFadeController.FadeState.kFadeIn)
        {
            EventFadeIn = event_fade;
        }
        else
        {
            EventFadeOut = event_fade;
        }
    }

    public void Register(InputManager input)
    {
        MyInput = input;
    }

    public void Register(CinemachineManager cinemachine_manager)
    {
        Cinemachines = cinemachine_manager;
    }

    public void Register(Light sun_light)
    {
        SunLight = sun_light;
    }

    public void Register(StageLoader stage_loader)
    {
        MyStageLoader = stage_loader;
    }
}