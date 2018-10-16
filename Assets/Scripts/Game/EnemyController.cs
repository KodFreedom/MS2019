﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] float kTimeToAttack = 3f;
    public Animator MyAnimator { get; private set; }
    private static int kPlayerLayer;
    private float current_life_ = 3f;
    private PlayerController player_ = null;
    private float attack_timer_ = 0f;
    private GameObject punch_collider_ = null;

    public bool IsDead { get { return current_life_ == 0f; } }

    public void OnPlayerEntered(PlayerController player)
    {
        player_ = player;
    }

    public void OnBeginFight()
    {
        attack_timer_ = kTimeToAttack;
    }

    public void OnExitFight()
    {
        attack_timer_ = 0f;
    }

	private void Start ()
    {
        kPlayerLayer = LayerMask.NameToLayer("Player");
        transform.parent.GetComponent<BattleAreaController>().Register(this);
        MyAnimator = GetComponent<Animator>();
        punch_collider_ = transform.Find("PunchCollider").gameObject;
    }
	
	private void Update ()
    {
        if (current_life_ <= 0f) return;

        if (player_)
        {
            TurnToPlayer();
            Attack();
        }
	}

    private void LateUpdate()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == kPlayerLayer)
        {
            if(other.gameObject.name.Equals("PunchCollider"))
            {
                HitByPunch();
            }
            else if (other.gameObject.name.Equals("UltraCollider"))
            {
                HitByUltra();
            }
        }
    }

    private void TurnToPlayer()
    {
        transform.LookAt(player_.transform, Vector3.up);
    }

    private void Attack()
    {
        if(attack_timer_ > 0f)
        {
            attack_timer_ -= Time.deltaTime;
            
            if(MyAnimator.GetBool("IsHitted") == true
                || MyAnimator.GetBool("IsAttack") == true)
            {
                attack_timer_ = kTimeToAttack;
            }

            if(attack_timer_ <= 0f)
            {
                MyAnimator.SetBool("IsAttack", true);
                attack_timer_ = kTimeToAttack;
            }
        }

        punch_collider_.SetActive(MyAnimator.GetFloat("EnablePunchCollider") == 1f);
    }

    private void HitByPunch()
    {
        player_.OnPunchHit();

        current_life_ -= player_.Attack;

        if (current_life_ <= 0f)
        {
            current_life_ = 0f;
            MyAnimator.SetBool("IsDead", true);
        }
        else
        {
            MyAnimator.SetBool("IsHitted", true);
        }
    }

    private void HitByUltra()
    {
        //player_.OnPunchHit();

        current_life_ = 0f;
        MyAnimator.SetBool("IsDead", true);
    }
}
