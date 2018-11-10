using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultData : MonoBehaviour
{
    public void OnGameClear()
    {
        // Todo : Collect data
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    private void Start()
    {
        GameObject.DontDestroyOnLoad(gameObject);
        GameManager.Instance.Data.Register(this);
    }
}
