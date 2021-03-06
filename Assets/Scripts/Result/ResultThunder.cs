﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResultThunder : MonoBehaviour
{
    private enum State
    {
        kShowText,
        kUpdateThunder,
    }

    [SerializeField] TextMeshProUGUI kText = null;
    [SerializeField] TextMeshProUGUI kCounter = null;
    [SerializeField] float kFadeTime = 0.2f;
    [SerializeField] float kShakeRange = 20f;
    [SerializeField] float kShakeTime = 0.5f;
    [SerializeField] float kAdditionalScale = 1f;
    private float timer_ = 0f;
    private State current_state_ = State.kShowText;

    // Use this for initialization
    void Start()
    {
        SetText(kText, 0f);
        SetText(kCounter, 0f);
    }

    public void Set(float amount)
    {
        kCounter.text = string.Format("{0:00000}", (int)amount);
    }

    public void Act(ResultPanel panel)
    {
        switch (current_state_)
        {
            case State.kShowText:
                ShowText(panel);
                break;
            case State.kUpdateThunder:
                UpdateThunder(panel);
                break;
        }
    }

    private void ShowText(ResultPanel panel)
    {
        timer_ += Time.deltaTime;
        float rate = Mathf.Clamp(timer_ / kFadeTime, 0f, 1f);
        SetText(kText, rate);
        SetText(kCounter, rate);
        if (timer_ >= kFadeTime)
        {
            timer_ = kFadeTime;
            panel.Shake(kShakeRange, kShakeTime);
            current_state_ = State.kUpdateThunder;
            SoundManager.Instance.PlaySe("Result_score000", false);
        }
    }

    private void SetText(TextMeshProUGUI text, float rate)
    {
        var face_color = text.faceColor;
        face_color.a = (byte)(255 * rate);
        text.faceColor = face_color;

        var glow_color = text.fontMaterial.GetColor(ShaderUtilities.ID_GlowColor);
        glow_color.a = rate * 0.5f;
        text.fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, glow_color);

        text.transform.localScale = Vector3.one * (1f + (1f - rate) * kAdditionalScale);
    }

    private void UpdateThunder(ResultPanel panel)
    {
        panel.OnResultEventOver();
    }
}
