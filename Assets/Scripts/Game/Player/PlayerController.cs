#define HaveJoyCon

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Playables;

public class PlayerController : MonoBehaviour
{
    private struct InputInfo
    {
        public bool left_punch;
        public bool right_punch;
        public bool thunder_mode;
        public bool ultra;
        public float left_charge;
        public float right_charge;
    }

    private struct VibrationFlag
    {
        public bool left_punch_hit;
        public bool right_punch_hit;
        public bool damaged;
        public bool ultra_hit;
    }

    public NavMeshAgent NavAgent { get; private set; }
    public Animator MyAnimator { get; private set; }
    public BattleAreaController BattleArea { get; set; }
    public EnemyController TargetEnemy { get; set; }
    public PlayerFieldNavigationState FieldNavigationState { get; private set; }
    public PlayerBattleNavigationState BattleNavigationState { get; private set; }
    public PlayerEventNavigationState EventNavigationState { get; private set; }
    public PlayerNormalMode NormalMode { get; private set; }
    public PlayerThunderMode ThunderMode { get; private set; }
    public PlayerParameter Parameter { get; private set; }
    public GameObject PunchCollider { get; private set; }
    public GameObject UltraCollider { get; private set; }
    public PlayableDirector UltraController { get; private set; }
    public float Attack { get { return current_mode_.Attack(this); } }
    public bool IsTunderMode { get { return input_info_.thunder_mode; } }
    public bool Ultra { get { return input_info_.ultra; } }
    public bool LeftPunch { get { return input_info_.left_punch; } }
    public bool RightPunch { get { return input_info_.right_punch; } }
    public bool IsPlayingEvent = false;
    public bool EnableUltraCollider = false;

    private PlayerIkController ik_controller_ = null;
    private PlayerNavigationState current_navigation_state_ = null;
    private PlayerMode current_mode_ = null;
    private InputInfo input_info_;
    private VibrationFlag vibration_flag_;

    [SerializeField] string kMode; // Debug表示
    [SerializeField] string kState; // Debug表示
    [SerializeField] TextMesh kUi = null;

    /// <summary>
    /// 移動ステートの切り替え
    /// </summary>
    /// <param name="next_state"></param>
    public void Change(PlayerNavigationState next_state)
    {
        if(current_navigation_state_ != null)
        {
            current_navigation_state_.Uninit(this);
            Debug.Log("Change : " + current_navigation_state_.Name());
        }

        Debug.Log("To : " + next_state.Name());
        kState = next_state.Name();
        current_navigation_state_ = next_state;
        current_navigation_state_.Init(this);
        kState = current_navigation_state_.Name();
    }

    /// <summary>
    /// 戦闘モードの切り替え
    /// </summary>
    /// <param name="next_mode"></param>
    public void Change(PlayerMode next_mode)
    {
        if (current_mode_ != null)
        {
            current_mode_.Uninit(this);
            Debug.Log("Change : " + current_mode_.Name());
        }

        Debug.Log("To : " + next_mode.Name());
        kMode = next_mode.Name();
        current_mode_ = next_mode;
        current_mode_.Init(this);
    }

    public void OnPunchHit()
    {
        var current_vcam = Camera.main.GetComponent<Cinemachine.CinemachineBrain>().ActiveVirtualCamera;
        var camera_shake = current_vcam.VirtualCameraGameObject.GetComponent<CameraShake>();
        camera_shake.Shake(Parameter.PunchCameraShakeRange, Parameter.PunchCameraShakeTime);

        if(MyAnimator.GetCurrentAnimatorStateInfo(0).IsName("LeftPunch"))
        {
            vibration_flag_.left_punch_hit = true;
        }
        else
        {
            vibration_flag_.right_punch_hit = true;
        }
    }

    public void OnUltraHit()
    {
        var current_vcam = Camera.main.GetComponent<Cinemachine.CinemachineBrain>().ActiveVirtualCamera;
        var camera_shake = current_vcam.VirtualCameraGameObject.GetComponent<CameraShake>();
        camera_shake.Shake(Parameter.UltraCameraShakeRange, Parameter.UltraCameraShakeTime);
        vibration_flag_.ultra_hit = true;
    }

    public void OnDamaged()
    {
        var current_vcam = Camera.main.GetComponent<Cinemachine.CinemachineBrain>().ActiveVirtualCamera;
        var camera_shake = current_vcam.VirtualCameraGameObject.GetComponent<CameraShake>();
        camera_shake.Shake(Parameter.KnockbackCameraShakeRange, Parameter.KnockbackCameraShakeTime);
        vibration_flag_.damaged = true;
    }

    private void Start ()
    {
        GameManager.Instance.Data.Register(this);

        MyAnimator = GetComponent<Animator>();
        ik_controller_ = GetComponent<PlayerIkController>();
        NavAgent = GetComponent<NavMeshAgent>();
        Parameter = GetComponent<PlayerParameter>();
        PunchCollider = transform.Find("PunchCollider").gameObject;
        PunchCollider.SetActive(false);
        UltraCollider = transform.Find("UltraCollider").gameObject;
        UltraCollider.SetActive(false);

        UltraController = GetComponent<PlayableDirector>();
        foreach (var at in UltraController.playableAsset.outputs)
        {
            if (at.streamName.Equals("Cinemachine"))
            {
                UltraController.SetGenericBinding(at.sourceObject, Camera.main.GetComponent<Cinemachine.CinemachineBrain>());
            }
        }

        UltraController.Stop();
        IsPlayingEvent = false;
        EnableUltraCollider = false;

        FieldNavigationState = new PlayerFieldNavigationState();
        BattleNavigationState = new PlayerBattleNavigationState();
        EventNavigationState = new PlayerEventNavigationState();
        EventNavigationState.SetNextState(FieldNavigationState);

        NormalMode = new PlayerNormalMode();
        ThunderMode = new PlayerThunderMode();

        Change(NormalMode);
        Change(EventNavigationState);
        GameManager.Instance.ChangeStage();
    }
	
	private void Update ()
    {
        Time.timeScale = Parameter.kTimeScale;
        Parameter.Tick(Time.deltaTime);
        UpdateInput();
        UpdateCharge();
        UpdateVibration();
        current_navigation_state_.Update(this);
        current_mode_.Update(this);
        UpdateUi();
    }

    private void OnTriggerEnter(Collider other)
    {
        current_navigation_state_.OnTriggerEnter(this, other);
        current_mode_.OnTriggerEnter(this, other);
    }

    private void OnTriggerExit(Collider other)
    {
        current_navigation_state_.OnTriggerExit(this, other);
        current_mode_.OnTriggerExit(this, other);
    }

    // 入力
    private void UpdateInput()
    {
        var input = GameManager.Instance.MyInput;
        input_info_.left_punch = input.GetPunchL();
        input_info_.right_punch = input.GetPunchR();
        input_info_.ultra = input.GetSpecialSkill();
        input_info_.thunder_mode = input.GetThunderMode();
        input_info_.left_charge = input.GetGyroL().y + (Input.GetKey(KeyCode.LeftArrow) ? -10f : 0f);
        input_info_.right_charge = -input.GetGyroR().y + (Input.GetKey(KeyCode.RightArrow) ? 10f : 0f);
    }

    // 振動
    private void UpdateVibration()
    {
        var input = GameManager.Instance.MyInput;
        if (MyAnimator.GetBool("OnLeftPunchEnter"))
        {
            input.VibrationPunchiShotL();
        }

        if (MyAnimator.GetBool("OnRightPunchEnter"))
        {
            input.VibrationPunchiShotR();
        }

        if(vibration_flag_.left_punch_hit)
        {
            vibration_flag_.left_punch_hit = false;
            input.VibrationPunchiHitL();
        }

        if (vibration_flag_.right_punch_hit)
        {
            vibration_flag_.right_punch_hit = false;
            input.VibrationPunchiHitR();
        }

        if (vibration_flag_.damaged)
        {
            vibration_flag_.damaged = false;
            input.VibrationDamage();
        }

        if(vibration_flag_.ultra_hit)
        {
            vibration_flag_.ultra_hit = false;
            input.VibrationSkill();
        }
    }

    // 充電
    private void UpdateCharge()
    {
        bool enable_charge_ = MyAnimator.GetFloat("EnableCharge") == 1f ? true : false;
        enable_charge_ = UltraController.state == PlayState.Playing ? false : enable_charge_;

        ik_controller_.SetActive(enable_charge_);

        if (enable_charge_)
        {
            ik_controller_.RotateLeft(-input_info_.left_charge);
            ik_controller_.RotateRight(-input_info_.right_charge);
            Parameter.ChangeEnergy(Parameter.ChargeSpeed * Time.deltaTime
                * (Mathf.Abs(input_info_.left_charge) + Mathf.Abs(input_info_.right_charge)));
        }
    }

    // Ui
    private void UpdateUi()
    {
        kUi.text = "Energy : " + Parameter.CurrentEnergy.ToString("000") + " / " + Parameter.MaxEnergy.ToString("000");
        kUi.text += "\nTime : " + Parameter.Timer.ToString("000.00");
    }
}
