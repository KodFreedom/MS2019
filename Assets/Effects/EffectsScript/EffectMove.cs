using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectMove : MonoBehaviour 
{
    public bool bMoveXUse;
    public bool bMoveUpUse;
    public bool bMoveZUse;
    public bool bRotationZUse;
    public float fMoveXValue;
    public float fMoveUpValue;
    public float fMoveZValue;
    public float fRotationXValue;
	// Use this for initialization
	void Start () 
    {
		
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (bMoveXUse)
        {
            MoveX(fMoveXValue);
        }

        if (bMoveUpUse)
        {
            MoveUp(fMoveUpValue);
        }

        if (bMoveZUse)
        {
            MoveZ(fMoveZValue);
        }

        if (bRotationZUse)
        {
            RotationZ(fRotationXValue);
        }

    }

    void MoveX(float PosX)
    {
        this.transform.position += new Vector3(PosX, 0.0f, 0.0f);
    }

    void MoveUp(float PosY)
    {
        this.transform.position -= new Vector3(0.0f, PosY, 0.0f);
    }

    void MoveZ(float PosZ)
    {
        this.transform.position -= new Vector3(0.0f, 0.0f, PosZ);
    }

    void RotationZ(float RotZ)
    {
        this.transform.Rotate(new Vector3(0.0f, 0.0f, RotZ));
    }

}
