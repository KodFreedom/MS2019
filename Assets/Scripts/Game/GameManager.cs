using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance_ = null;
    public static GameManager Instance { get { return instance_; } }

    public GameData Data { get; private set; }
    public InputManager MyInput { get; private set; }
    private StageLoader stage_loader_ = null;

    public void StageClear()
    {
        stage_loader_.OnStageClear();
    }

    public void ChangeStage()
    {
        stage_loader_.OnStageChange();
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