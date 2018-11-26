using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogoMove : MonoBehaviour
{
    //変数宣言
    public bool bPlus;
    public bool bMinus;
    Vector3 AddPos;
    float MoveValue;
    public float MaxValue;
    public GameObject ActiveTarget;
    public GameObject ActiveTarget2;

    // Use this for initialization
    void Start ()
    {
        MoveValue = 0.0f;
        AddPos.x = 2.0f;
	}
	
	// Update is called once per frame
	void Update ()
    {
        MoveValue += AddPos.x;

        if (bPlus)
        {
            this.transform.position += new Vector3(AddPos.x, 0.0f, 0.0f);
        }

        if (bMinus)
        {
            this.transform.position -= new Vector3(AddPos.x, 0.0f, 0.0f);
        }

        if (MoveValue >= MaxValue)
        {
            bPlus = false;
            bMinus = false;
        }

        if (!bMinus)
        {
            ActiveTarget.SetActive(true);
            if (Startfade.GetStartfadeAlfa() <= 0)
            {
                ActiveTarget2.SetActive(true);
            }
        }
    }
}
