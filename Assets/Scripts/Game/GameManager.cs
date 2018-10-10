using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance_ = null;
    public static GameManager Instance { get { return instance_; } }

    private GameData data_ = null;
    public GameData Data { get { return data_; } }

    private StageLoader stage_loader_ = null;

    public void StageClear()
    {
        stage_loader_.OnStageClear();
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

        data_ = new GameData();
        stage_loader_ = GetComponent<StageLoader>();
        stage_loader_.Init();
    }
}