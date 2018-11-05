using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargedParticle : MonoBehaviour 
{

    //変数宣言
    ParticleSystem.ShapeModule ShapParticle;            //ShapeModuleの値を変更するための変数
    ParticleSystem.EmissionModule EmissionParticle;     //EmissionModuleの値を変更するための変数
    ParticleSystem.MainModule MainParticle;             //MainModuleの値を変更をするための変数
    public float StartSpeed;                            //任意の方向への各パーティクルの初速
    public float StartLifeTime;                         //パーティクルの最初の生存期間
    public float RateOverTime;                          //ユニットタイム毎に放出されるパーティクルの数
    public float AddRadius;                             //形状の円形面の半径に加算していく値
    public float MaxRadius;                             //形状の円形面の半径の最大値 
    public float AddStartSpeed;                         //任意の方向への各パーティクルの初速に加算していく値
    public float MaxStartSpeed;                         //任意の方向への各パーティクルの初速の最大値
    public float AddStartLifeTime;                      //パーティクルの最初の生存期間に加算していく値
    public float MaxStartLifeTime;                      //パーティクルの最初の生存期間の最大値
    public float AddRateOverTime;                       //ユニットタイム毎に放出されるパーティクルの数に加算していく値
    public float MaxRateOverTime;                       //ユニットタイム毎に放出されるパーティクルの数の最大値
    public float Power;
    public float MaxPower;

    void Start()
    {
        ParticleSystem ParticleObj = this.GetComponent<ParticleSystem>();        //オブジェクトから ParticleSystemコンポーネントを取得 
        EmissionParticle = ParticleObj.emission;                                 //アクセスするために必要なemissionを取得して格納
        ShapParticle = ParticleObj.shape;                                        //アクセスするために必要なshapeを取得して格納
        MainParticle = ParticleObj.main;                                         //アクセスするために必要なMainModuleを取得して格納
        MainParticle.startSpeed = StartSpeed;                                    //Inspectorで入力した値を代入
        MainParticle.startLifetime = StartLifeTime;                              //Inspectorで入力した値を代入
        EmissionParticle.rateOverTime = RateOverTime;                            //Inspectorで入力した値を代入
        //Power = 0;
    }

    void Update()
    {
        //Shapeモジュールのradiusの値変更
        if (ShapParticle.radius >= MaxRadius)
        {
            ShapParticle.radius = MaxRadius;
        }
        ShapParticle.radius += AddRadius;

        //StartSpeedの値の変更
        if (StartSpeed >= MaxStartSpeed)
        {
            StartSpeed = MaxStartSpeed;
        }
        StartSpeed += AddStartSpeed;
        MainParticle.startSpeed = StartSpeed;

        //StartLifeの値の変更
        if (StartLifeTime >= MaxStartLifeTime)
        {
            StartLifeTime = MaxStartLifeTime;
        }
        StartLifeTime += AddStartLifeTime;
        MainParticle.startLifetime = StartLifeTime;

        //RateOverTimeの値の変更
        if (RateOverTime >= MaxRateOverTime)
        {
            RateOverTime = MaxRateOverTime;
        }
        //Powerの値の変更
        if (Power >= MaxPower)
        {
            Power = MaxPower;
        }
        RateOverTime = Power / 2;
        EmissionParticle.rateOverTime = RateOverTime;
    }
}
