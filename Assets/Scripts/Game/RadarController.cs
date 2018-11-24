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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (other.gameObject.name.Equals("UltraCollider"))
            {
                GameManager.Instance.Data.Player.OnUltraHit();
            }
        }
    }
}
