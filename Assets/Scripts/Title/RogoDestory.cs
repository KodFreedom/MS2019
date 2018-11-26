using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogoDestory : MonoBehaviour
{
    public float timeOut;
    private float timeElapsed;
    public GameObject Ac;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= 1.47f)
        {
            Ac.SetActive(true);

            Destroy(this.gameObject);
        }
    }
}
