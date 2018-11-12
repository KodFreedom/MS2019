using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameData Data { get; private set; }
    public InputManager MyInput { get; private set; }
    public CinemachineManager Cinemachines { get; private set; }
    public EventFadeController EventFadeIn { get; private set; }
    public EventFadeController EventFadeOut { get; private set; }
    public Light SunLight { get; private set; }
    
    private StageLoader stage_loader_ = null;
    private bool game_clear_ = false;

    public bool IsLastStage()
    {
        return stage_loader_.NextStage == null;
    }

    public void GameClear()
    {
        if (game_clear_) return;
        game_clear_ = true;
        Data.Player.IsPlayingEvent = true;
        Data.Result.OnGameClear();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Result");
    }

    public void StageClear()
    {
        stage_loader_.OnStageClear();
    }

    public void ChangeStage()
    {
        if (IsLastStage())
        {
            GameClear();
        }
        else
        {
            stage_loader_.OnStageChange();
        }
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

    private void OnDestroy()
    {
        if(Instance == this)
        {
            Instance = null;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Data = new GameData();
        SunLight = GetComponentInChildren<Light>();
        Cinemachines = GetComponent<CinemachineManager>();
        stage_loader_ = GetComponent<StageLoader>();
        stage_loader_.Init();
    }

    private void Start()
    {
        MyInput = JoyconManager.Instance.gameObject.GetComponent<InputManager>();
    }
}