using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageLoader : MonoBehaviour
{
    [SerializeField] GameObject[] kStages = new GameObject[1];
    public GameObject NextStage { get; private set; }
    private GameObject current_stage_ = null;
    private int counter_ = 0;

    public void OnStageClear()
    {
        current_stage_.GetComponent<StageController>().PrepareClearEvent();
    }

    public void OnStageChange()
    {
        if (current_stage_)
        {
            Destroy(current_stage_);
            current_stage_ = null;
        }

        current_stage_ = NextStage;
        NextStage = LoadNextStage();

        var stage_controller = current_stage_.GetComponent<StageController>();
        stage_controller.PrepareStartEvent();
        InitBattleAreas(stage_controller);
    }

    private void Start()
    {
        NextStage = LoadNextStage();
    }

    private GameObject LoadNextStage()
    {
        if (counter_ == kStages.Length)
        {
            return null;
        }

        GameObject result = Instantiate<GameObject>(kStages[counter_]);
        result.GetComponent<StageController>().Init();
        counter_++;
        return result;
    }

    private void InitBattleAreas(StageController stage)
    {
        stage.BattleAreas.SetActive(true);
        var battle_areas = stage.BattleAreas.GetComponentsInChildren<BattleAreaController>();
        List<BattleAreaController> result = new List<BattleAreaController>();
        foreach(var battle_area in battle_areas)
        {
            result.Add(battle_area);
        }
        GameManager.Instance.Data.Register(result);
    }
}