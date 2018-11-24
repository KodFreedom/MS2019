using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingDestroy : MonoBehaviour 
{
    [SerializeField] float Life = 3;
    public float CurrentLifeRate { get { return TimeElapsed / Life; } }
    private float TimeElapsed = 0f;

    // Use this for initialization
    void Start ()
    {
		
	}

    // Update is called once per frame
    void Update()
    {
        TimeElapsed += Time.deltaTime;

        if (TimeElapsed >= Life)
        {
            Destroy(this.gameObject);
        }
    }
}
