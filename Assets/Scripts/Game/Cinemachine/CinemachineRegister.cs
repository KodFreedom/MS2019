using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemachineRegister : MonoBehaviour
{
	void Start ()
    {
        var cinemachine = GetComponent<Cinemachine.CinemachineVirtualCameraBase>();
        if(cinemachine)
        {
            GameManager.Instance.Data.Cinemachines.Register(cinemachine);
        }
	}
}
