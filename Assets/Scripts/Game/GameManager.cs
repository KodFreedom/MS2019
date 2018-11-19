using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameData Data { get; private set; }
    public bool GetReady { get; private set; }
    private bool game_clear_ = false;

    public bool IsLastStage()
    {
        return Data.MyStageLoader.NextStage == null;
    }

    public void GameClear()
    {
        if (game_clear_) return;
        game_clear_ = true;
        Data.Player.IsPlayingEvent = true;
        Data.Result.OnGameClear();
    }

    public void StageClear()
    {
        Data.MyStageLoader.OnStageClear();
    }

    public void ChangeStage()
    {
        if (IsLastStage())
        {
            GameClear();
        }
        else
        {
            Data.MyStageLoader.OnStageChange();
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
        Data.Register(GetComponentInChildren<Light>());
        Data.Register(GetComponent<CinemachineManager>());
        Data.Register(GetComponent<StageLoader>());
        GetReady = false;
    }

    private void Start()
    {
        Data.Register(JoyconManager.Instance.gameObject.GetComponent<InputManager>());
    }

    private void Update()
    {
        if (GetReady) return;
        if(Data.GetReady())
        {
            GetReady = true;
            Data.Player.ReadyToStart();
        }
    }
}