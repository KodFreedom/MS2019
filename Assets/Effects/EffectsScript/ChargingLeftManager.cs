using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingLeftManager : MonoBehaviour 
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
        GameObject CloneObject = Instantiate(Effect, new Vector3(-1.0f, 0.0f, 0.0f), Quaternion.identity);
        //CloneObject.transform.localScale = new Vector3(0.00006f, 0.00006f, 0.00006f);
        CloneObject.transform.parent = this.transform;
        CloneObject.transform.position = this.transform.position;
    }
}
