using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLogo : MonoBehaviour
{
    Vector3 AddScal;
    Vector3 MaxScal;
    Vector3 MinScal;
    bool bUse;

	// Use this for initialization
	void Start ()
    {
        AddScal = new Vector3(0.005f, 0.005f, 0.000f);
        MaxScal = new Vector3(0.6f, 0.6f, 0.0f);
        MinScal = new Vector3(0.3f, 0.3f, 0.0f);
        bUse = true;
    }

    // Update is called once per frame
    void Update ()
    {
        Transform myTransform = this.transform;

        if(bUse)
        {
            this.transform.localScale += AddScal;
        }
        else
        {
            this.transform.localScale -= AddScal;
        }

        if (myTransform.localScale.x >= MaxScal.x && myTransform.localScale.y >= MaxScal.y)
        {
            bUse = false;  
        }

        if (myTransform.localScale.x <= MinScal.x && myTransform.localScale.y <= MinScal.y)
        {
            bUse = true;
        }

        if(ImagePanel.GetIsBreak())
        {
            Destroy(this.gameObject);
        }
    }

}
