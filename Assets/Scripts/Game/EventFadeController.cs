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
    private Image fade_image_ = null;
    private float time_counter_ = -1f;

	// Use this for initialization
	private void Start ()
    {
        fade_image_ = GetComponent<Image>();
        fade_image_.color = new Color(1f, 1f, 1f, 0f);
        GameManager.Instance.Register(this, kState);


        if(kState == FadeState.kFadeOut)
        {
            gameObject.SetActive(false);
        }
        else
        {
            time_counter_ = 0f;
        }
    }

    private void OnEnable()
    {
        time_counter_ = 0f;
    }

    private void OnDisable()
    {
        time_counter_ = -1f;
        if (fade_image_ == null) return;
        if (kState == FadeState.kFadeOut)
        {
            fade_image_.color = new Color(1f, 1f, 1f, 0f);
        }
        else
        {
            fade_image_.color = new Color(1f, 1f, 1f, 1f);
        }
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
        fade_image_.color = new Color(1f, 1f, 1f, 1f - time_counter_ / kFadeTime);
        time_counter_ += Time.deltaTime;
        if (time_counter_ >= kFadeTime)
        {
            time_counter_ = kFadeTime;
        }
    }

    private void FadeOut()
    {
        fade_image_.color = new Color(1f, 1f, 1f, time_counter_ / kFadeTime);
        time_counter_ += Time.deltaTime;
        if(time_counter_ >= kFadeTime)
        {
            time_counter_ = kFadeTime;
        }
    }
}
