﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResultRank : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI kText = null;
    [SerializeField] float kFadeTime = 0.2f;
    [SerializeField] float kShakeRange = 20f;
    [SerializeField] float kShakeTime = 0.5f;
    [SerializeField] float kAdditionalScale = 1f;
    private float timer_ = 0f;

    // Use this for initialization
    void Start()
    {
        SetText(kText, 0f);
    }

    public void Set(string rank)
    {
        kText.text = rank;
    }

    public void Act(ResultPanel panel)
    {
        ShowText(panel);
    }

    private void ShowText(ResultPanel panel)
    {
        timer_ += Time.deltaTime;
        float rate = Mathf.Clamp(timer_ / kFadeTime, 0f, 1f);
        SetText(kText, rate);
        if (timer_ >= kFadeTime)
        {
            timer_ = kFadeTime;
            panel.Shake(kShakeRange, kShakeTime);
            panel.OnResultEventOver();
            SoundManager.Instance.PlaySe("Result_score001", false);
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
}
