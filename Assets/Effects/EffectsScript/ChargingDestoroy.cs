using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingDestoroy : MonoBehaviour 
{
    public float Life;
    private float TimeElapsed;

    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        TimeElapsed += Time.deltaTime;

        if (TimeElapsed >= Life)
        {
            Destroy(this.gameObject);
            TimeElapsed = 0.0f;
        }
    }
}
