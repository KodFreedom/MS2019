using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageLoader : MonoBehaviour
{
    [SerializeField] GameObject[] kStages = new GameObject[1];
    private GameObject previous_stage_ = null;
    private GameObject current_stage_ = null;
    private GameObject next_stage_ = null;
    private int counter_ = 0;

    public void OnStageClear()
    {
        current_stage_.GetComponent<StageController>().PrepareClearEvent();
    }

    public void OnStageChange()
    {
        if (previous_stage_)
        {
            Destroy(previous_stage_);
        }

        previous_stage_ = current_stage_;
        current_stage_ = next_stage_;
        next_stage_ = LoadNextStage();

        current_stage_.GetComponent<StageController>().PrepareStartEvent();
        InitBattleAreas(current_stage_);
    }

    public void Init()
    {
        current_stage_ = LoadNextStage();
        next_stage_ = LoadNextStage();
        InitBattleAreas(current_stage_);
    }

    private GameObject LoadNextStage()
    {
        if (counter_ == kStages.Length)
        {
            return null;
        }

        GameObject result = Instantiate<GameObject>(kStages[counter_]);
        counter_++;

        return result;
    }

    private void InitBattleAreas(GameObject stage)
    {
        if (stage == null) return;

        var battle_areas = stage.transform.Find("BattleAreas").GetComponentsInChildren<BattleAreaController>();
        List<BattleAreaController> result = new List<BattleAreaController>();
        foreach(var battle_area in battle_areas)
        {
            result.Add(battle_area);
        }
        GameManager.Instance.Data.Register(result);
    }
}