using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultCinemachineRegister : MonoBehaviour
{
	void Start ()
    {
        var cinemachine = GetComponent<Cinemachine.CinemachineVirtualCameraBase>();
        if (cinemachine)
        {
            GameManager.Instance.Data.Cinemachines.RegisterResultCamera(cinemachine);
        }
    }
}
