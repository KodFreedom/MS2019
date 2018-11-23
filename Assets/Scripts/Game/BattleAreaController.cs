using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BattleAreaController : MonoBehaviour
{
    public List<EnemyController> Enemies { get; private set; }
    public bool Paused { get; private set; }
    private PlayerController player_ = null;
    private PlayableDirector enter_battle_event_ = null;
    private bool playing_enter_event_ = false;
    Dictionary<string, PlayableBinding> binding_dictionary_ = new Dictionary<string, PlayableBinding>();

    public void OnBattleStart(PlayerController player)
    {
        player_ = player;
        foreach(var enemy in Enemies)
        {
            enemy.OnPlayerEntered(player);
        }

        StartEvent();
        Paused = false;
    }

    public void OnBattleResume()
    {
        Debug.Log("OnBattleResume");
        Paused = false;
        foreach (var enemy in Enemies)
        {
            enemy.OnPlayerEntered(player_);
        }
    }

    public void OnBattlePause()
    {
        Debug.Log("OnBattlePause");
        Paused = true;
        foreach (var enemy in Enemies)
        {
            enemy.OnPlayerExited();
        }
    }

    public void OnBattleAreaClear()
    {
        Destroy(gameObject, 15f);
    }

    public void Register(EnemyController enemy)
    {
        Enemies.Add(enemy);
    }

    /// <summary>
    /// 最も近いの敵を返す
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public EnemyController GetNearestEnemy(Vector3 position)
    {
        EnemyController result = null;
        float max_square_distance = float.MaxValue;

        foreach(var enemy in Enemies)
        {
            if(enemy.IsDead == false)
            {
                float square_distance = Vector3.SqrMagnitude(position - enemy.transform.position);
                if (square_distance < max_square_distance)
                {
                    max_square_distance = square_distance;
                    result = enemy;
                }
            }
        }

        return result;
    }

    private void Start()
    {
        Enemies = new List<EnemyController>();
        enter_battle_event_ = GetComponent<PlayableDirector>();
        playing_enter_event_ = false;

        if (enter_battle_event_)
        {
            foreach (var at in enter_battle_event_.playableAsset.outputs)
            {
                if (!binding_dictionary_.ContainsKey(at.streamName))
                {
                    binding_dictionary_.Add(at.streamName, at);
                }
            }
        }
    }

    private void Update()
    {
        if (enter_battle_event_ && playing_enter_event_ == true)
        {
            if (enter_battle_event_.state == PlayState.Paused)
            {
                player_.IsPlayingEvent = false;
                playing_enter_event_ = false;
            }
        }
    }

    private void StartEvent()
    {
        if(enter_battle_event_)
        {
            playing_enter_event_ = true;
            player_.EventNavigationState.SetWaitTime(0.5f);
            player_.IsPlayingEvent = true;
            foreach (var enemy in Enemies)
            {
                enemy.SetWaitTime(0.5f);
            }
            enter_battle_event_.SetGenericBinding(binding_dictionary_["Player"].sourceObject, player_.MyAnimator);
            enter_battle_event_.Play();
        }
    }
}
