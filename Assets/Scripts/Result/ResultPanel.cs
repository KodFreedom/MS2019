using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultPanel : MonoBehaviour
{
    private enum State
    {
        kWait,
        kOpenPanel,
        kShowTime,
        kShowThunder,
        kShowRank,
        kOver
    }

    [SerializeField] float kPanelTimeCost = 0.5f;
    [SerializeField] float kWaitTime = 0.5f;
    private State current_state_ = State.kWait;
    private Vector3 panel_origin_ = Vector3.zero;
    private Vector3 shake_offset_ = Vector3.zero;
    private float max_shake_range_ = 0f;
    private float max_shake_time_ = 0f;
    private float shake_timer_ = 0f;
    private float wait_time_ = 0f;
    private ResultTime result_time_ = null;
    private ResultThunder result_thunder_ = null;
    private ResultRank result_rank_ = null;

    public void Shake(float max_range, float max_time)
    {
        if (max_shake_time_ != 0f) return;
        SetParameter(max_range, max_time, 0f);
    }

    public void OnResultEventOver()
    {
        ++current_state_;
        wait_time_ = kWaitTime;
    }

    public void Act()
    {
        // Get Data
        result_time_.Set(233f);
        result_thunder_.Set(233f);
        result_rank_.Set("A");
        current_state_ = State.kOpenPanel;
    }

    private void Start()
    {
        transform.localScale = new Vector3(1, 0, 1);
        panel_origin_ = transform.localPosition;
        result_time_ = GetComponent<ResultTime>();
        result_thunder_ = GetComponent<ResultThunder>();
        result_rank_ = GetComponent<ResultRank>();
    }

    private void Update()
    {
        UpdateShake();

        if (wait_time_ > 0f)
        {
            wait_time_ -= Time.deltaTime;
            return;
        }

        switch (current_state_)
        {
            case State.kWait:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Act();
                }
                break;
            case State.kOpenPanel:
                OpenPanel();
                break;
            case State.kShowTime:
                result_time_.Act(this);
                break;
            case State.kShowThunder:
                result_thunder_.Act(this);
                break;
            case State.kShowRank:
                result_rank_.Act(this);
                break;
        }
    }

    private void UpdateShake()
    {
        if (max_shake_time_ > 0f)
        {
            float rate = (max_shake_time_ - shake_timer_) / max_shake_time_;

            shake_timer_ += Time.deltaTime;
            if (shake_timer_ > max_shake_time_)
            {
                SetParameter(0f, 0f, 0f);
                rate = 0f;
            }

            shake_offset_ = GetOffset() * rate;
        }

        transform.localPosition = panel_origin_ + shake_offset_;
    }

    private void OpenPanel()
    {
        var scale = transform.localScale;
        scale.y += Time.deltaTime / kPanelTimeCost;
        if(scale.y >= 1f)
        {
            scale.y = 1f;
            current_state_ = State.kShowTime;
            wait_time_ = kWaitTime;
        }
        transform.localScale = scale;
    }

    private void SetParameter(float max_range, float max_time, float timer)
    {
        max_shake_time_ = max_time;
        max_shake_range_ = max_range;
        shake_timer_ = timer;
    }

    private Vector3 GetOffset()
    {
        return new Vector3(
            Random.Range(-max_shake_range_, max_shake_range_),
            Random.Range(-max_shake_range_, max_shake_range_),
            0f);
    }
}
