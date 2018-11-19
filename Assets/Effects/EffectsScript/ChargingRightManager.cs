using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingRightManager : MonoBehaviour
{
    public float CreateTime;
    private float TimeElapsed;
    public GameObject Effect;
    // Use this for initialization
    void Start()
    {
        Create();

    }

    // Update is called once per frame
    void Update()
    {
        TimeElapsed += Time.deltaTime;

        if (TimeElapsed >= CreateTime)
        {
            Create();
            TimeElapsed = 0.0f;
        }
    }

    void Create()
    {
        Instantiate(Effect, Effect.transform.position, Effect.transform.rotation);
    }
}