﻿using System.Collections;
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
        public bool SetRotation;
        public Transform target;
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
    [SerializeField] EventPlayerInfo kStartEventPlayerInfo;
    [SerializeField] EventPlayerInfo kClearEventPlayerInfo;
    [SerializeField] float kPrepareTime = 0.2f;
    [SerializeField] Material kSkyBox = null;
    [SerializeField] Vector3 kLightDirection = Vector3.zero;
    [SerializeField] Color kLightColor = Color.white;
    [SerializeField] GameObject kModels = null;
    [SerializeField] GameObject kBattleAreas = null;
    [SerializeField] string kBgm;
    public GameObject BattleAreas { get { return kBattleAreas; } }
    private Dictionary<string, PlayableBinding> binding_dictionary_start_ = new Dictionary<string, PlayableBinding>();
    private Dictionary<string, PlayableBinding> binding_dictionary_clear_ = new Dictionary<string, PlayableBinding>();
    private EventState start_event_state_ = EventState.kStopped;
    private EventState clear_event_state_ = EventState.kStopped;
    private float prepare_time_counter_ = 0f;
    private Vector3 origin_position_ = Vector3.zero;
    private Vector3 origin_rotation_ = Vector3.zero;

    public void PrepareStartEvent()
    {
        if (start_event_state_ == EventState.kPreparing) return;

        Debug.Log(gameObject.name + "PrepareStartEvent");
        start_event_state_ = EventState.kPreparing;

        if(kBgm.Length > 0)
        {
            SoundManager.Instance.PlayBgm(kBgm);
        }

        if(GameManager.Instance.IsLastStage())
        {
            Debug.Log("SetLedLightDown");
            GameManager.Instance.Data.MyInput.SetLedLightDown();
        }

        kModels.SetActive(true);
        var game_data = GameManager.Instance.Data;
        var player = game_data.Player;
        player.EventNavigationState.SetWaitTime(0.5f);
        player.IsPlayingEvent = true;
        player.MyAnimator.applyRootMotion = true;
        prepare_time_counter_ = 0f;
        origin_position_ = player.transform.position;
        origin_rotation_ = player.transform.rotation.eulerAngles;

        if(kStageStartEvent)
        {
            if(binding_dictionary_start_.ContainsKey("Player"))
            {
                kStageStartEvent.SetGenericBinding(binding_dictionary_start_["Player"].sourceObject, player.MyAnimator);
            }

            if (binding_dictionary_start_.ContainsKey("EventFadeIn"))
            {
                kStageStartEvent.SetGenericBinding(binding_dictionary_start_["EventFadeIn"].sourceObject, game_data.EventFadeIn.gameObject);
            }

            if (binding_dictionary_start_.ContainsKey("EventFadeOut"))
            {
                kStageStartEvent.SetGenericBinding(binding_dictionary_start_["EventFadeOut"].sourceObject, game_data.EventFadeOut.gameObject);
            }

            if (binding_dictionary_start_.ContainsKey("Cinemachine"))
            {
                kStageStartEvent.SetGenericBinding(binding_dictionary_start_["Cinemachine"].sourceObject, Camera.main.GetComponent<CinemachineBrain>());
                var cinemachine_track = binding_dictionary_start_["Cinemachine"].sourceObject as Cinemachine.Timeline.CinemachineTrack;
                foreach (var clip in cinemachine_track.GetClips())
                {
                    var cinemachine_shot = clip.asset as Cinemachine.Timeline.CinemachineShot;
                    var camera = game_data.Cinemachines.GetBy(clip.displayName);
                    if (camera)
                    {
                        var vcam = camera;
                        var set_cam = new ExposedReference<CinemachineVirtualCameraBase>();
                        set_cam.defaultValue = vcam;
                        cinemachine_shot.VirtualCamera = set_cam;
                    }
                }
            }
        }

        if (kSkyBox)
        {
            RenderSettings.skybox = kSkyBox;
            game_data.SunLight.transform.localRotation = Quaternion.Euler(kLightDirection);
            game_data.SunLight.color = kLightColor;
        }
    }

    public void PrepareClearEvent()
    {
        if (clear_event_state_ == EventState.kPreparing) return;

        Debug.Log(gameObject.name + "PrepareClearEvent");
        clear_event_state_ = EventState.kPreparing;

        var game_data = GameManager.Instance.Data;
        var player = game_data.Player;
        player.EventNavigationState.SetWaitTime(0.5f);
        player.IsPlayingEvent = true;
        player.MyAnimator.applyRootMotion = true;
        prepare_time_counter_ = 0f;
        origin_position_ = player.transform.position;
        origin_rotation_ = player.transform.rotation.eulerAngles;

        if (kStageClearEvent)
        {
            if (binding_dictionary_clear_.ContainsKey("Player"))
            {
                kStageClearEvent.SetGenericBinding(binding_dictionary_clear_["Player"].sourceObject, player.MyAnimator);
            }

            if (binding_dictionary_clear_.ContainsKey("EventFadeOut"))
            {
                kStageClearEvent.SetGenericBinding(binding_dictionary_clear_["EventFadeOut"].sourceObject, game_data.EventFadeOut.gameObject);
            }

            if (binding_dictionary_clear_.ContainsKey("Cinemachine"))
            {
                // MainCamera
                kStageClearEvent.SetGenericBinding(binding_dictionary_clear_["Cinemachine"].sourceObject, Camera.main.GetComponent<CinemachineBrain>());

                // CameraShot
                var cinemachine_track = binding_dictionary_clear_["Cinemachine"].sourceObject as Cinemachine.Timeline.CinemachineTrack;
                foreach (var clip in cinemachine_track.GetClips())
                {
                    Debug.Log(clip.displayName);
                    var cinemachine_shot = clip.asset as Cinemachine.Timeline.CinemachineShot;
                    var camera = game_data.Cinemachines.GetBy(clip.displayName);
                    if (camera)
                    {
                        var vcam = camera;
                        var set_cam = new ExposedReference<CinemachineVirtualCameraBase>();
                        set_cam.defaultValue = vcam;
                        cinemachine_shot.VirtualCamera = set_cam;
                    }
                }
            }

            var hip = GameManager.Instance.Data.Player.transform.Find("chara_rigged:skelton1").Find("chara_rigged:hip").gameObject;

            if (binding_dictionary_clear_.ContainsKey("ChargeEffect"))
            {
                var control_track = binding_dictionary_clear_["ChargeEffect"].sourceObject as UnityEngine.Timeline.ControlTrack;
                var set_obj = new ExposedReference<GameObject>();
                set_obj.defaultValue = hip;
                foreach (var clip in control_track.GetClips())
                {
                    Debug.Log(clip.displayName);
                    var control_playable_asset = clip.asset as UnityEngine.Timeline.ControlPlayableAsset;
                    control_playable_asset.sourceGameObject = set_obj;
                }
            }

            if (binding_dictionary_clear_.ContainsKey("ExploreEffect"))
            {
                var control_track = binding_dictionary_clear_["ExploreEffect"].sourceObject as UnityEngine.Timeline.ControlTrack;
                var set_obj = new ExposedReference<GameObject>();
                set_obj.defaultValue = hip;
                foreach (var clip in control_track.GetClips())
                {
                    Debug.Log(clip.displayName);
                    var control_playable_asset = clip.asset as UnityEngine.Timeline.ControlPlayableAsset;
                    control_playable_asset.sourceGameObject = set_obj;
                }
            }
        }
    }

    public void Init()
    {
        kModels.SetActive(false);
        kBattleAreas.SetActive(false);

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
                        kStageClearEvent.Play();
                    }
                    break;
                }
            case EventState.kPlaying:
                {
                    if (kStageClearEvent)
                    {
                        if ((kStageClearEvent.extrapolationMode != DirectorWrapMode.Hold && kStageClearEvent.state != PlayState.Playing)
                            || (kStageClearEvent.extrapolationMode == DirectorWrapMode.Hold
                            && kStageClearEvent.state == PlayState.Playing
                            && kStageClearEvent.duration == kStageClearEvent.time))
                        {
                            OnClearEventStopping();
                        }

                        if (GameManager.Instance.IsLastStage()
                            && kStageClearEvent.extrapolationMode == DirectorWrapMode.Hold
                            && GameManager.Instance.Data.Cinemachines.ResultCamera)
                        {
                            GameManager.Instance.Data.Cinemachines.ResultCamera.Priority = 11;
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
            var position = Vector3.Slerp(origin_position_
                , kStartEventPlayerInfo.target.position, prepare_time_counter_ / kPrepareTime);
            player.transform.position = position;
        }
        if (kStartEventPlayerInfo.SetRotation)
        {
            var rotation = Vector3.Slerp(origin_rotation_
                , kStartEventPlayerInfo.target.rotation.eulerAngles, prepare_time_counter_ / kPrepareTime);
            player.transform.rotation = Quaternion.Euler(rotation);
        }
        
        if(prepare_time_counter_ > kPrepareTime)
        {
            start_event_state_ = EventState.kStarting;
        }
        prepare_time_counter_ += Time.deltaTime;
    }

    private void OnClearEventPreparing()
    {
        if (prepare_time_counter_ > kPrepareTime)
        {
            clear_event_state_ = EventState.kStarting;
        }

        var player = GameManager.Instance.Data.Player;
        if (kClearEventPlayerInfo.SetPosition)
        {
            player.transform.position = Vector3.Slerp(origin_position_
                , kClearEventPlayerInfo.target.position, prepare_time_counter_ / kPrepareTime);
        }
        if (kClearEventPlayerInfo.SetRotation)
        {
            var rotation = Vector3.Slerp(origin_rotation_
                , kClearEventPlayerInfo.target.rotation.eulerAngles, prepare_time_counter_ / kPrepareTime);
            player.transform.rotation = Quaternion.Euler(rotation);
        }
        prepare_time_counter_ += Time.deltaTime;
    }

    private void OnStartEventStopping()
    {
        Debug.Log(gameObject.name + "OnStartEventStopping");
        start_event_state_ = EventState.kStopped;
        var player = GameManager.Instance.Data.Player;
        //player.MyAnimator.applyRootMotion = false;
        player.IsPlayingEvent = false;
    }

    private void OnClearEventStopping()
    {
        Debug.Log(gameObject.name + "OnClearEventStopping");
        if (kBgm.Length > 0)
        {
            SoundManager.Instance.StopBgm(kBgm);
        }
        clear_event_state_ = EventState.kStopped;
        var player = GameManager.Instance.Data.Player;
        //player.MyAnimator.applyRootMotion = false;
        GameManager.Instance.ChangeStage();
    }
}
