using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowItemController : MonoBehaviour
{
    [SerializeField] float kSpeed = 1f;
    [SerializeField] float kLife = 3f;
    [SerializeField] float kGravityMultiplier = 0.5f;
    private Vector3 direction_ = Vector3.zero;
    private float gravity_multiplier_ = 0f;

    public void Act(PlayerController target)
    {
        transform.parent = null;
        direction_ = (target.Parameter.Head.position - transform.position).normalized;
        gravity_multiplier_ = kGravityMultiplier;
        Destroy(gameObject, kLife);
    }

    public void OnReflect()
    {
        direction_ *= -2f;
        direction_ += Vector3.up;
    }

    public void OnClear()
    {
        Destroy(gameObject);
    }

    private void Update ()
    {
        transform.position += (direction_ * kSpeed + Physics.gravity * gravity_multiplier_) * Time.deltaTime;
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            GetComponent<Collider>().enabled = false;
        }
    }
}
