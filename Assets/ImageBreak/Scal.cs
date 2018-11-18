using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scal : MonoBehaviour
{
    private float timeElapsed;
    BoxCollider m_ObjectCollider;

    public Transform Direction;

    Rigidbody rb;
    public Vector3 Pos;

    // Use this for initialization
    void Start ()
    {
        rb = GetComponent<Rigidbody>();
        m_ObjectCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update ()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= 1.0f)
        {
            m_ObjectCollider.enabled = false;
        }

        //this.transform.localScale -= new Vector3(0.004f, 0.004f, 0.004f);
        //this.transform.Rotate(new Vector3(0.1f, 0.1f, 0.1f));
        rb.AddForce(Pos.x, Pos.y, Pos.z);
    }
    private void OnDrawGizmos()
    {
        if (Direction == null)
        {
            return;
        }
        Gizmos.DrawLine(transform.position, Direction.position);
    }
}
