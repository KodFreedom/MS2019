using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private float current_life_ = 3f;
    private PlayerController player_ = null;
    private Animator animator_ = null;

    public bool IsDead { get { return current_life_ == 0f; } }

    public void OnPlayerEntered(PlayerController player)
    {
        player_ = player;
    }

    public void OnBeginFight()
    {

    }

	private void Start ()
    {
        transform.parent.GetComponent<BattleAreaController>().Register(this);
        animator_ = GetComponent<Animator>();
	}
	
	private void Update ()
    {
        if (current_life_ <= 0f) return;

        if (player_)
        {
            transform.LookAt(player_.transform, Vector3.up);
        }
	}

    private void LateUpdate()
    {
        animator_.SetBool("IsHitted", false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name.Equals("PunchCollider"))
        {
            current_life_ -= player_.Attack;
            if(current_life_ <= 0f)
            {
                current_life_ = 0f;
                animator_.SetBool("IsDead", true);
            }
            else
            {
                animator_.SetBool("IsHitted", true);
            }
        }
    }
}
