using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventFadeController : MonoBehaviour
{
    [System.Serializable]
    public enum FadeState
    {
        kFadeIn,
        kFadeOut,
    }

    [SerializeField] float kFadeTime = 1f;
    [SerializeField] FadeState kState = FadeState.kFadeIn;
    [SerializeField] Material kFadeMaterial = null;
    private float time_counter_ = -1f;
    private float rate_ = 0f;

	// Use this for initialization
	private void Start ()
    {
        GameManager.Instance.Data.Register(this, kState);
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        time_counter_ = 0f;
        rate_ = kState == FadeState.kFadeOut ? 0f : 1f;
        kFadeMaterial.SetFloat("_Rate", rate_);
    }

    private void OnDisable()
    {
        time_counter_ = -1f;
        //rate_ = kState == FadeState.kFadeOut ? 0f : 1f;
        //fade_material_.SetFloat("_Rate", rate_);
    }

    // Update is called once per frame
    private void Update ()
    {
        if (time_counter_ < 0f) return;

        switch (kState)
        {
            case FadeState.kFadeIn:
                FadeIn();
                break;
            case FadeState.kFadeOut:
                FadeOut();
                break;
            default:
                break;
        }
	}

    private void FadeIn()
    {
        rate_ = 1f - time_counter_ / kFadeTime;
        kFadeMaterial.SetFloat("_Rate", rate_);
        time_counter_ += Time.deltaTime;
        if (time_counter_ >= kFadeTime)
        {
            time_counter_ = kFadeTime;
        }
    }

    private void FadeOut()
    {
        rate_ = time_counter_ / kFadeTime;
        kFadeMaterial.SetFloat("_Rate", rate_);
        time_counter_ += Time.deltaTime;
        if(time_counter_ >= kFadeTime)
        {
            time_counter_ = kFadeTime;
        }
    }
}
