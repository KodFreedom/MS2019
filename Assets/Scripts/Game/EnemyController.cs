using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] float Life = 3f;
    [SerializeField] float kStopDistance = 0.5f;
    [SerializeField] float kPunchCoolDown = 3f;
    [SerializeField] float kThrowCoolDown = 3f;
    public Animator MyAnimator { get; private set; }
    private int player_layer_ = 0;
    private PlayerController target_ = null;
    private GameObject punch_collider_ = null;
    private NavMeshAgent agent_ = null;
    private float punch_timer_ = 0f;
    private float wait_time_ = 0f;

    public bool IsDead { get { return Life == 0f; } }

    public void SetWaitTime(float time)
    {
        wait_time_ = time;
    }

    public void OnPlayerEntered(PlayerController player)
    {
        target_ = player;
        MyAnimator.applyRootMotion = false;
        punch_timer_ = punch_timer_ > 0f ? kPunchCoolDown : 0f;
    }

    public void OnPlayerExited()
    {
        target_ = null;
    }

    public void OnBeginFight()
    {
        agent_.enabled = true;
        agent_.SetDestination(target_.transform.position + target_.transform.forward * kStopDistance);
    }

	private void Start ()
    {
        player_layer_ = LayerMask.NameToLayer("Player");
        transform.parent.GetComponent<BattleAreaController>().Register(this);
        MyAnimator = GetComponent<Animator>();
        agent_ = GetComponent<NavMeshAgent>();
        agent_.enabled = false;
        punch_collider_ = transform.Find("PunchCollider").gameObject;
    }
	
	private void Update ()
    {
        punch_collider_.SetActive(MyAnimator.GetFloat("EnablePunchCollider") > 0.5f);

        if (Life <= 0f) return;
        if (wait_time_ > 0f)
        {
            wait_time_ -= Time.deltaTime;
            return;
        }

        if (target_)
        {
            if (target_.IsPlayingEvent) return;
            Navigation();
            Punch();
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == player_layer_)
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

    private void Navigation()
    {
        if (agent_.enabled == true)
        {
            if (CheckArrive())
            {
                agent_.enabled = false;
                punch_timer_ = kPunchCoolDown;
            }
        }
        else
        {// Turn to player
            var direction = Vector3.Scale(target_.transform.position - transform.position, new Vector3(1, 0, 1)).normalized;
            transform.forward = Vector3.Slerp(transform.forward, direction, 0.5f);
        }
    }

    private void Punch()
    {
        if(punch_timer_ > 0f)
        {
            punch_timer_ -= Time.deltaTime;
            
            if(MyAnimator.GetBool("EnableAttack") == false)
            {
                punch_timer_ = kPunchCoolDown;
            }

            if(punch_timer_ <= 0f)
            {
                MyAnimator.SetBool("IsAttack", true);
                punch_timer_ = kPunchCoolDown;
            }
        }
    }

    private void HitByPunch()
    {
        if (agent_.enabled == true || target_ == null) return;

        target_.OnPunchHit();

        Life -= target_.Attack;

        if (Life <= 0f)
        {
            Life = 0f;
            MyAnimator.SetBool("IsDead", true);
        }
        else
        {
            MyAnimator.SetBool("IsHitted", true);
        }
    }

    private void HitByUltra()
    {
        agent_.enabled = false;
        GameManager.Instance.Data.Player.OnUltraHit();
        Life = 0f;
        MyAnimator.SetBool("IsDead", true);
    }

    private bool CheckArrive()
    {
        // Check if we've reached the destination
        if (!agent_.pathPending)
        {
            if (agent_.remainingDistance <= agent_.stoppingDistance)
            {
                if (!agent_.hasPath || agent_.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }
}