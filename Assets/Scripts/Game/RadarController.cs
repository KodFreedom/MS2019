using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarController : MonoBehaviour
{
    [SerializeField] float kRotateSpeed = 60f;
    private Vector3 rotation_ = Vector3.zero;

    private void Start()
    {
        rotation_ = transform.localRotation.eulerAngles;
    }

    // Update is called once per frame
    private void Update ()
    {
        rotation_.y += kRotateSpeed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(rotation_);
    }
}
