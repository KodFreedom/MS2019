using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialSphere : MonoBehaviour 
{
    //変数宣言
    public float life_time;         //何秒後に爆発するか
    float time;                     //現在の時間
    Vector3 Scal;                   //オブジェクトの大きさ
    public Vector3 AddScal;
    public Vector3 MaxScal;

    // Use this for initialization
    void Start()
    {
        //初期化
        Scal = new Vector3(1.0f, 1.0f, 1.0f);
        time = 0;
        AddScal = AddScal;
    }

    // Update is called once per frame
    void Update()
    {
        //徐々にスケールを大きくしていく
        Scal.x += AddScal.x;
        Scal.y += AddScal.y;
        Scal.z += AddScal.z;

        if (Scal.x >= MaxScal.x)
        {
            Scal.x = MaxScal.x;
        }

        if (Scal.y >= MaxScal.y)
        {
            Scal.y = MaxScal.y;
        }

        if (Scal.z >= MaxScal.z)
        {
            Scal.z = MaxScal.z;
        }
        this.transform.localScale = new Vector3(Scal.x, Scal.y, Scal.z);

        //this.transform.Rotate(0.0f, 0.5f, 0.0f);

        time += Time.deltaTime;
        if (time > life_time)
        {
            Destroy(gameObject);
        }
    }
}
