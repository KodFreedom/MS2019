using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightActive : MonoBehaviour
{
    public GameObject ActiveObj;
    float ActiveTime;
    float Interbal;
    bool bInterbalTime;
    bool bActiveTime;

    // Use this for initialization
    void Start ()
    {
        ActiveTime = Random.Range(0.1f, 0.7f);
        Interbal = 0.0f;
        bInterbalTime = false;
        bActiveTime = true;

    }
	
	// Update is called once per frame
	void Update ()
    {
        if(bActiveTime)
        {
            if (ActiveTime >= 0.0f)
            {
                ActiveTime -= Time.deltaTime;

                ActiveObj.SetActive(true);
            }
            else
            {
                ActiveObj.SetActive(false);
                bInterbalTime = true;
                bActiveTime = false;
                Interbal = Random.Range(0.1f, 0.7f);
            }
        }

        if(bInterbalTime)
        {
            if (Interbal > 0.0f)
            {
                Interbal -= Time.deltaTime;
            }
            else
            {
                bInterbalTime = false;
                bActiveTime = true;
                ActiveTime = Random.Range(0.1f, 0.7f);
            }
        }

        if (ImagePanel.GetIsBreak())
        {
            Destroy(this.gameObject);
        }
    }
}
