using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingController : MonoBehaviour
{
    [SerializeField] float kLife = 3f;
    private float current_life_ = 0f;
    private Vector3 origin_ = Vector3.zero;

    private void Start()
    {
        origin_ = transform.localPosition;
    }

    private void Update()
    {
        Move();
        CountDestroy();
    }

    private void Move()
    {
        transform.localPosition = Vector3.Lerp(origin_, Vector3.zero, current_life_ / kLife);
    }

    private void CountDestroy()
    {
        if (current_life_ >= kLife)
        {
            Destroy(gameObject);
        }
        current_life_ += Time.deltaTime;
    }
}
