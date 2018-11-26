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
    [SerializeField] GameObject kThrowItem = null;
    [SerializeField] Transform kThrowHand = null;
    public Animator MyAnimator { get; private set; }
    private int player_layer_ = 0;
    private PlayerController target_ = null;
    private GameObject punch_collider_ = null;
    private ThrowItemController throw_item_ = null;
    private NavMeshAgent agent_ = null;
    private float punch_timer_ = 0f;
    private float throw_timer_ = 0f;
    private float wait_time_ = 0f;
    private int se_number_ = 0;

    public bool IsDead { get { return Life <= 0f; } }

    public void SetWaitTime(float time)
    {
        wait_time_ = time;
    }

    public void OnPlayerEntered(PlayerController player)
    {
        target_ = player;
        MyAnimator.applyRootMotion = false;
        punch_timer_ = punch_timer_ > 0f ? kPunchCoolDown : 0f;
        throw_timer_ = throw_timer_ >= 0f ? kThrowCoolDown : -1f;
    }

    public void OnPlayerExited()
    {
        target_ = null;
    }

    public void OnBeginFight()
    {
        throw_timer_ = -1f;
        DestroyThrowItem();
        agent_.enabled = true;
        agent_.SetDestination(target_.transform.position + target_.transform.forward * kStopDistance);
        MyAnimator.CrossFade("Run", 0.2f);
        SoundManager.Instance.PlaySe("Game_walk000", true);
    }

	private void Start ()
    {
        player_layer_ = LayerMask.NameToLayer("Player");
        transform.parent.GetComponent<BattleAreaController>().Register(this);
        MyAnimator = GetComponent<Animator>();
        agent_ = GetComponent<NavMeshAgent>();
        agent_.enabled = false;
        punch_collider_ = transform.Find("PunchCollider").gameObject;
        se_number_ = Random.Range(0, 3);
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
            Throw();
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
                MyAnimator.CrossFade("Idle", 0.2f);
                SoundManager.Instance.StopLoopSe("Game_walk000", 0f);
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
                MyAnimator.Play("Punch" + Random.Range(0, 2).ToString());
                punch_timer_ = kPunchCoolDown;
            }
        }
    }

    private void Throw()
    {
        if(throw_item_)
        {
            if(MyAnimator.GetFloat("EnableThrow") > 0.5f)
            {
                throw_item_.Act(target_);
                throw_item_ = null;
            }
        }

        if (throw_timer_ > 0f)
        {
            throw_timer_ -= Time.deltaTime;

            if (MyAnimator.GetBool("EnableAttack") == false)
            {
                throw_timer_ = kThrowCoolDown;
            }

            if (throw_timer_ <= 0f)
            {
                MyAnimator.Play("Throw");
                throw_item_ = Instantiate(kThrowItem, kThrowHand).GetComponent<ThrowItemController>();
                throw_timer_ = kThrowCoolDown;
            }
        }
    }

    private void DestroyThrowItem()
    {
        if(throw_item_)
        {
            Destroy(throw_item_.gameObject);
            throw_item_ = null;
        }
    }

    private void HitByPunch()
    {
        if (agent_.enabled == true || target_ == null || Life <= 0f) return;

        target_.OnPunchHit();
        DestroyThrowItem();
        Life -= target_.Attack;
        if (Life <= 0f)
        {
            Life = 0f;
            MyAnimator.Play("Dying");
            SoundManager.Instance.PlaySe("Game_EnmyLose00" + se_number_, false);
        }
        else
        {
            MyAnimator.CrossFade("Hit", 0.2f);
            SoundManager.Instance.PlaySe("Game_EnmyDmg00" + se_number_, false);
        }
    }

    private void HitByUltra()
    {
        if (Life <= 0f) return;
        GameManager.Instance.Data.Player.OnUltraHit();
        agent_.enabled = false;
        DestroyThrowItem();
        Life = 0f;
        MyAnimator.Play("Dying");
        SoundManager.Instance.PlaySe("Game_EnmyLose00" + se_number_, false);
        SoundManager.Instance.StopLoopSe("Game_walk000", 0f);
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