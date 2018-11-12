using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultData : MonoBehaviour
{
    private Transform canvas_ = null;

    public void OnGameClear()
    {
        // Todo : Collect data
        canvas_.gameObject.SetActive(true);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    private void Start()
    {
        GameObject.DontDestroyOnLoad(gameObject);
        GameManager.Instance.Data.Register(this);
        canvas_ = transform.GetChild(0);
        canvas_.gameObject.SetActive(false);
    }
}
