using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultController : MonoBehaviour
{
    [SerializeField] float kTimeToChange = 30f;
    [SerializeField] float kFadeTime = 0.5f;
    [SerializeField] Image fade_out_ = null;
    private AsyncOperation title_scene_ = null;
    private float time_counter_ = 0f;
    private ResultData data_ = null;

    public void OnGameClear()
    {
        if (time_counter_ > 0f) return;
        data_.Act();
        title_scene_ = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Title");
        title_scene_.allowSceneActivation = false;
        time_counter_ = kTimeToChange + kFadeTime;
    }

    private void Start ()
    {
        data_ = GetComponent<ResultData>();
        GameManager.Instance.Data.Register(this);
    }

    private void Update()
    {
        ToTitleScene();
    }

    private void ToTitleScene()
    {
        var color = fade_out_.color;
        if (color.a >= 1f)
        {
            title_scene_.allowSceneActivation = true;
        }

        if (time_counter_ > 0f)
        {
            time_counter_ -= Time.deltaTime;
            if (time_counter_ <= kFadeTime)
            {
                color.a += Time.deltaTime / kFadeTime;
                fade_out_.color = color;
            }
        }
    }
}
