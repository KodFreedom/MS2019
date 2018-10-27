using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance_ = null;
    public static GameManager Instance { get { return instance_; } }

    public GameData Data { get; private set; }
    public InputManager MyInput { get; private set; }
    public EventFadeController EventFadeIn { get; private set; }
    public EventFadeController EventFadeOut { get; private set; }
    private StageLoader stage_loader_ = null;
    private bool game_clear_ = false;

    public void GameClear()
    {
        if (game_clear_) return;
        Data.Player.IsPlayingEvent = true;
        Data.Result.Run();
    }

    public void StageClear()
    {
        stage_loader_.OnStageClear();
    }

    public void ChangeStage()
    {
        stage_loader_.OnStageChange();
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

    private void Awake()
    {
        if (instance_ == null)
        {
            instance_ = this;
        }
        else
        {
            Destroy(this);
        }

        Data = new GameData();
        MyInput = GetComponent<InputManager>();
        stage_loader_ = GetComponent<StageLoader>();
        stage_loader_.Init();
    }
}