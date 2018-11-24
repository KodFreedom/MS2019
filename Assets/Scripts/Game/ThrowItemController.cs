using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowItemController : MonoBehaviour
{
    [SerializeField] float kSpeed = 1f;
    [SerializeField] float kLife = 3f;
    private Vector3 direction_ = Vector3.zero;

    public void Act(PlayerController target)
    {
        transform.parent = null;
        direction_ = (target.transform.position - transform.position).normalized;
        Destroy(gameObject, kLife);
    }

	private void Update ()
    {
        transform.position += direction_ * kSpeed * Time.deltaTime;
	}
}
