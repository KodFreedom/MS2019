using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GageController : MonoBehaviour {
    
    public GameObject ElectricalObject;


    void Start () {
		
	}
	
	void Update () {

        float value = ElectricalObject.GetComponent<ElectricalValue>().GetEnergy();

        

        if (value > 500.0f)
            value = 500.0f;

        GetComponent<RectTransform>().sizeDelta = new Vector2(30.0f, value);
    }
}
