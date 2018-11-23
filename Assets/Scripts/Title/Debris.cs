using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debris : MonoBehaviour
{
    //変数宣言
    [Tooltip("破片を飛ばしたい向き")]
    public Transform Direction;
    [Tooltip("加える力の大きさ")]
    public Vector3 AddPower;
    Rigidbody rb;
    Vector3 SkipForward;

    // Use this for initialization
    void Start ()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update ()
    {
        GameObject.DontDestroyOnLoad(this);
        //transform.parent = GameObject.Find("Canvas").transform;

        //this.transform.localScale -= new Vector3(0.004f, 0.004f, 0.004f);
        this.transform.Rotate(new Vector3( 0.0f, 0.0f, 0.0f));
        SkipForward = Vector3.Scale(Direction.position - transform.position, new Vector3(1, 1, 1)).normalized;
        rb.AddForce(SkipForward.x * AddPower.x, SkipForward.y * AddPower.y, SkipForward.z * AddPower.z, ForceMode.VelocityChange);
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
