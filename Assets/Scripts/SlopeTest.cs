using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlopeTest : MonoBehaviour {

    public GameObject InputManagerObject;

    private float gyroTest = 0.0f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        InputManager inputManager = InputManagerObject.GetComponent<InputManager>();
        
        Vector3 joyconGyro = inputManager.GetGyroL();
        Quaternion rcqt = transform.rotation;

        //rcqt = Quaternion.Euler(joyconGyro) * rcqt;

        Vector3 vec = rcqt.eulerAngles;

        /*vec.x += joyconGyro.z;
        vec.y += -joyconGyro.x;
        vec.z += -joyconGyro.y;*/

        vec.x += joyconGyro.x;
        vec.y += joyconGyro.y;
        vec.z += joyconGyro.z;

        //vec.x += 1.0f;

        transform.rotation = Quaternion.Euler(vec);

        Debug.Log(transform.rotation.eulerAngles);
        
    }
}
