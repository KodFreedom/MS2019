﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeController : MonoBehaviour
{
    //変数宣言
    public GameObject ActiveTarget;
    public float timeOut;
    private float timeElapsed;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= timeOut)
        {
            ActiveTarget.SetActive(true);
        }
    }
}
