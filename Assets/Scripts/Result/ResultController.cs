using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultController : MonoBehaviour
{
    private Text text_ = null;

    public void Run()
    {
        text_.enabled = true;
    }

	private void Start ()
    {
        text_ = GetComponentInChildren<Text>();
        text_.enabled = false;
	}
}
