using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IlluminAwakerController : MonoBehaviour
{
    public float Radius = 0f;
    public float Strength = 1f;

    private void Start()
    {
        Radius = 0f;
        Strength = 1f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
}
