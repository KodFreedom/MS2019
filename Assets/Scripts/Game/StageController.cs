using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;

public class StageController : MonoBehaviour
{
    [System.Serializable]
    private struct EventPlayerInfo
    {
        public bool SetPosition;
        public Vector3 Position;
        public bool SetRotation;
        public Vector3 Rotation;
    }

    private enum EventState
    {
        kPreparing,
        kStarting,
        kPlaying,
        kStopped
    }

    [SerializeField] PlayableDirector kStageStartEvent;
    [SerializeField] PlayableDirector kStageClearEvent;
    private Dictionary<string, PlayableBinding> binding_dictionary_start_ = new Dictionary<string, PlayableBinding>();
    private Dictionary<string, PlayableBinding> binding_dictionary_clear_ = new Dictionary<string, PlayableBinding>();
    private EventState start_event_state_ = EventState.kStopped;
    private EventState clear_event_state_ = EventState.kStopped;
    [SerializeField] EventPlayerInfo kStartEventPlayerInfo;
    [SerializeField] EventPlayerInfo kClearEventPlayerInfo;
    [SerializeField] float kPrepareTime = 0.2f;
    private float prepare_time_counter_ = 0f;

    public void PrepareStartEvent()
    {
        Debug.Log(gameObject.name + "PrepareStartEvent");
        start_event_state_ = EventState.kPreparing;
        var player = GameManager.Instance.Data.Player;
        player.EventNavigationState.SetWaitTime(0.5f);
        player.IsPlayingEvent = true;
        player.MyAnimator.applyRootMotion = true;
        prepare_time_counter_ = 0f;

        if(kStageStartEvent)
        {
            kStageStartEvent.SetGenericBinding(binding_dictionary_clear_["Player"].sourceObject, player.MyAnimator);
        }
    }

    public void PrepareClearEvent()
    {
        Debug.Log(gameObject.name + "PrepareClearEvent");
        clear_event_state_ = EventState.kPreparing;
        var player = GameManager.Instance.Data.Player;
        player.EventNavigationState.SetWaitTime(0.5f);
        player.IsPlayingEvent = true;
        player.MyAnimator.applyRootMotion = true;
        prepare_time_counter_ = 0f;

        if(kStageClearEvent)
        {
            kStageClearEvent.SetGenericBinding(binding_dictionary_clear_["Player"].sourceObject, player.MyAnimator);
            kStageClearEvent.SetGenericBinding(binding_dictionary_clear_["Cinemachine"].sourceObject, Camera.main.GetComponent<CinemachineBrain>());
            var cinemachine_track = binding_dictionary_clear_["Cinemachine"].sourceObject as Cinemachine.Timeline.CinemachineTrack;
            foreach (var clip in cinemachine_track.GetClips())
            {
                Debug.Log(clip.displayName);
                var cinemachine_shot = clip.asset as Cinemachine.Timeline.CinemachineShot;
                var camera = GameObject.Find(clip.displayName);
                if (camera)
                {
                    var vcam = camera.GetComponent<CinemachineVirtualCameraBase>();
                    var set_cam = new ExposedReference<CinemachineVirtualCameraBase>();
                    set_cam.defaultValue = vcam;
                    cinemachine_shot.VirtualCamera = set_cam;
                }
            }
        }
    }

    private void Start()
    {
        if (kStageStartEvent)
        {
            foreach (var at in kStageStartEvent.playableAsset.outputs)
            {
                if (!binding_dictionary_start_.ContainsKey(at.streamName))
                {
                    binding_dictionary_start_.Add(at.streamName, at);
                }
            }
        }

        if (kStageClearEvent)
        {
            foreach (var at in kStageClearEvent.playableAsset.outputs)
            {
                if (!binding_dictionary_clear_.ContainsKey(at.streamName))
                {
                    binding_dictionary_clear_.Add(at.streamName, at);
                }
            }
        }
    }

    private void LateUpdate()
    {
        UpdateStartEvent();
        UpdateClearEvent();
    }

    private void UpdateStartEvent()
    {
        switch (start_event_state_)
        {
            case EventState.kPreparing:
                {
                    OnStartEventPreparing();
                    break;
                }
            case EventState.kStarting:
                {
                    start_event_state_ = EventState.kPlaying;
                    if (kStageStartEvent)
                    {
                        kStageStartEvent.Play();
                    }
                    break;
                }
            case EventState.kPlaying:
                {
                    if (kStageStartEvent)
                    {
                        if (kStageStartEvent.state != PlayState.Playing)
                        {
                            OnStartEventStopping();
                        }
                    }
                    else
                    {
                        OnStartEventStopping();
                    }
                    break;
                }
            case EventState.kStopped:
                break;
        }
    }

    private void UpdateClearEvent()
    {
        switch (clear_event_state_)
        {
            case EventState.kPreparing:
                {
                    OnClearEventPreparing();
                    break;
                }
            case EventState.kStarting:
                {
                    clear_event_state_ = EventState.kPlaying;
                    if (kStageClearEvent)
                    {
                        Debug.Log(kStageClearEvent.name + " Start");
                        kStageClearEvent.Play();
                    }
                    break;
                }
            case EventState.kPlaying:
                {
                    if (kStageClearEvent)
                    {
                        if (kStageClearEvent.state != PlayState.Playing)
                        {
                            OnClearEventStopping();
                        }
                    }
                    else
                    {
                        OnClearEventStopping();
                    }
                    break;
                }
            case EventState.kStopped:
                break;
        }
    }

    private void OnStartEventPreparing()
    {
        var player = GameManager.Instance.Data.Player;
        if (kStartEventPlayerInfo.SetPosition)
        {
            player.transform.position = Vector3.Slerp(player.transform.position
                , kStartEventPlayerInfo.Position, prepare_time_counter_ / kPrepareTime);
        }
        if (kStartEventPlayerInfo.SetRotation)
        {
            var rotation = Vector3.Slerp(player.transform.rotation.eulerAngles
                , kStartEventPlayerInfo.Rotation, prepare_time_counter_ / kPrepareTime);
            player.transform.rotation = Quaternion.Euler(rotation);
        }
        prepare_time_counter_ += Time.deltaTime;
        if(prepare_time_counter_ > kPrepareTime)
        {
            start_event_state_ = EventState.kStarting;
        }
    }

    private void OnClearEventPreparing()
    {
        var player = GameManager.Instance.Data.Player;
        if (kClearEventPlayerInfo.SetPosition)
        {
            player.transform.position = Vector3.Slerp(player.transform.position
                , kClearEventPlayerInfo.Position, prepare_time_counter_ / kPrepareTime);
        }
        if (kClearEventPlayerInfo.SetRotation)
        {
            var rotation = Vector3.Slerp(player.transform.rotation.eulerAngles
                , kClearEventPlayerInfo.Rotation, prepare_time_counter_ / kPrepareTime);
            player.transform.rotation = Quaternion.Euler(rotation);
        }
        prepare_time_counter_ += Time.deltaTime;
        if (prepare_time_counter_ > kPrepareTime)
        {
            clear_event_state_ = EventState.kStarting;
        }
    }

    private void OnStartEventStopping()
    {
        Debug.Log(gameObject.name + "OnStartEventStopping");
        start_event_state_ = EventState.kStopped;
        var player = GameManager.Instance.Data.Player;
        player.MyAnimator.applyRootMotion = false;
        player.IsPlayingEvent = false;
    }

    private void OnClearEventStopping()
    {
        Debug.Log(gameObject.name + "OnClearEventStopping");
        clear_event_state_ = EventState.kStopped;
        var player = GameManager.Instance.Data.Player;
        player.MyAnimator.applyRootMotion = false;
        GameManager.Instance.ChangeStage();
    }
}
