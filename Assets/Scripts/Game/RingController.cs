using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingController : MonoBehaviour
{
    [SerializeField] ParticleSystem kLeftRing = null;
    [SerializeField] ParticleSystem kRightRing = null;
    [SerializeField] float kAbleUltraEffectFrequency = 0.2f;
    private Gradient color_over_lifetime_gradient_ = null;
    private GradientAlphaKey[] color_over_lifetime_alpha_keys_ = null;
    private GradientColorKey[] color_over_lifetime_color_keys_ = null;
    private float able_untra_effect_counter_ = 0f;

    private void Start()
    {
        color_over_lifetime_gradient_ = new Gradient();
        color_over_lifetime_gradient_.mode = GradientMode.Fixed;
        color_over_lifetime_alpha_keys_ = new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) };
        color_over_lifetime_color_keys_ = new GradientColorKey[] { new GradientColorKey(Color.yellow, 0.0f), new GradientColorKey(Color.gray, 1.0f) };
    }

    private void Update()
    {
        var parameter = GameManager.Instance.Data.Player.Parameter;
        UpdateAbleUltraEffect(parameter.CurrentEnergy > parameter.UltraCost);
        UpdateColorOverLifetime(parameter.CurrentEnergy / parameter.MaxEnergy);
    }

    private void UpdateColorOverLifetime(float rate)
    {
        color_over_lifetime_color_keys_[0].time = rate;
        color_over_lifetime_gradient_.SetKeys(color_over_lifetime_color_keys_, color_over_lifetime_alpha_keys_);
        SetRing(kLeftRing);
        SetRing(kRightRing);
    }

    private void UpdateAbleUltraEffect(bool able)
    {
        if (!able)
        {
            able_untra_effect_counter_ = 0f;
            return;
        }

        if (able_untra_effect_counter_ > kAbleUltraEffectFrequency)
        {
            able_untra_effect_counter_ = 0f;
        }

        var rate = 2f * able_untra_effect_counter_ / kAbleUltraEffectFrequency; // 0 - 2
        if (rate > 1f) rate = (rate - 1f) * -1f + 1f;

        color_over_lifetime_color_keys_[0].color = Color.yellow * Mathf.Lerp(0.5f, 1f, rate);

        able_untra_effect_counter_ += Time.deltaTime;
    }

    private void SetRing(ParticleSystem ring)
    {
        var color_over_life = ring.colorOverLifetime;
        color_over_life.color = color_over_lifetime_gradient_;
    }
}