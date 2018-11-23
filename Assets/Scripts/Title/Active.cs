using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Active : MonoBehaviour
{
    public GameObject ActiveObj;
	// Use this for initialization
	void Start ()
    {
        

    }
	
	// Update is called once per frame
	void Update ()
    {
        GameObject.DontDestroyOnLoad(this);

        if (ImagePanel.isBreak)
        {
            ActiveObj.SetActive(true);

        }
    }
}
