using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingManager : MonoBehaviour 
{
    [SerializeField] float CreateTime;
    [SerializeField] GameObject Effect;
    [SerializeField] bool IsPlaying = false;
    private float TimeElapsed;

    public void SetPlay(float value)
    {
        if(value > 0f)
        {
            IsPlaying = true;
        }
        else
        {
            IsPlaying = false;
            TimeElapsed = 0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsPlaying) return;

        TimeElapsed += Time.deltaTime;

        if (TimeElapsed >= CreateTime)
        {
            Create();
            TimeElapsed = 0.0f;
        }
    }

    void Create()
    {
        Instantiate(Effect, transform);
    }
}
