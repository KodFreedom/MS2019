using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StageTimerController : MonoBehaviour
{
    [SerializeField] TextMeshPro kText = null;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        var timer = GameManager.Instance.Data.Player.Parameter.Timer;
        kText.text = string.Format("{0:00}:{1:00}", (int)timer / 60, (int)timer % 60);
    }
}
