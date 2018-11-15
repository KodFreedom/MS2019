using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineManager : MonoBehaviour
{
    public CinemachineVirtualCameraBase ResultCamera { get; private set; }
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

    public void Register(CinemachineVirtualCameraBase cinemachine)
    {
        cinemachines_.Add(cinemachine.gameObject.name, cinemachine);
    }

    public void RegisterResultCamera(CinemachineVirtualCameraBase cinemachine)
    {
        ResultCamera = cinemachine;
    }
}
