using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineManager : MonoBehaviour
{
    private Dictionary<string, CinemachineVirtualCameraBase> cinemachines_ 
        = new Dictionary<string, CinemachineVirtualCameraBase>();

    public CinemachineVirtualCameraBase GetBy(string name)
    {
        CinemachineVirtualCameraBase result = null;

        if(cinemachines_.ContainsKey(name))
        {
            result = cinemachines_[name];
        }

        return result;
    }

    private void Awake()
    {
        var cinemachines = GetComponentsInChildren<CinemachineVirtualCameraBase>();
        foreach(var cinemachine in cinemachines)
        {
            cinemachines_.Add(cinemachine.gameObject.name, cinemachine);
        }
    }
}
