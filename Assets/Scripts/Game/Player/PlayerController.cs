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
        public bool ultra;
        public float left_charge;
        public float right_charge;
    }

    private struct VibrationFlag
    {
        public bool left_punch_enter;
        public bool right_punch_enter;
        public bool left_punch_hit;
        public bool right_punch_hit;
        public bool damaged;
        public bool ultra_hit;
    }

    public NavMeshAgent NavAgent { get; private set; }
    public Animator MyAnimator { get; private set; }
    public BattleAreaController BattleArea { get; set; }
    public EnemyController TargetEnemy { get; set; }
    public PlayerNavigationState CurrentNavigationState { get; private set; }
    public PlayerFieldNavigationState FieldNavigationState { get; private set; }
    public PlayerBattleNavigationState BattleNavigationState { get; private set; }
    public PlayerEventNavigationState EventNavigationState { get; private set; }
    public PlayerNormalMode NormalMode { get; private set; }
    public PlayerParameter Parameter { get; private set; }
    public GameObject PunchCollider { get; private set; }
    public GameObject UltraCollider { get; private set; }
    public PlayableDirector UltraController { get; private set; }
    public float Attack { get { return current_mode_.Attack(this); } }
    public bool Ultra { get { return input_info_.ultra; } }
    public bool LeftPunch { get { return input_info_.left_punch; } }
    public bool RightPunch { get { return input_info_.right_punch; } }
    public bool IsPlayingEvent = false;
    public bool EnableUltraCollider = false;
    public bool IsGameClear = false;

    private CustomIkController ik_controller_ = null;
    private PlayerMode current_mode_ = null;
    private InputInfo input_info_;
    private VibrationFlag vibration_flag_;

    [SerializeField] float kMinChargeAmount = 0.1f;

    /// <summary>
    /// 移動ステートの切り替え
    /// </summary>
    /// <param name="next_state"></param>
    public void Change(PlayerNavigationState next_state)
    {
        if(CurrentNavigationState != null)
        {
            CurrentNavigationState.Uninit(this);
            Debug.Log("Change : " + CurrentNavigationState.Name());
        }

        Debug.Log("To : " + next_state.Name());
        CurrentNavigationState = next_state;
        CurrentNavigationState.Init(this);
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
        current_mode_ = next_mode;
        current_mode_.Init(this);
    }

    public void OnLeftPunchEnter()
    {
        vibration_flag_.left_punch_enter = true;
    }

    public void OnRightPunchEnter()
    {
        vibration_flag_.right_punch_enter = true;
    }

    public void OnPunchHit()
    {
        if(MyAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Left"))
        {
            vibration_flag_.left_punch_hit = true;
        }
        else
        {
            vibration_flag_.right_punch_hit = true;
        }

        var current_vcam = Camera.main.GetComponent<Cinemachine.CinemachineBrain>().ActiveVirtualCamera;
        var camera_shake = current_vcam.VirtualCameraGameObject.GetComponent<CameraShake>();
        camera_shake.Shake(Parameter.PunchCameraShakeRange, Parameter.PunchCameraShakeTime);
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
        current_mode_.OnHitted(this);
    }

    public void ReadyToStart()
    {
        Change(NormalMode);
        Change(EventNavigationState);
        GameManager.Instance.ChangeStage();
    }

    private void Start ()
    {
        GameManager.Instance.Data.Register(this);

        MyAnimator = GetComponent<Animator>();
        ik_controller_ = GetComponent<CustomIkController>();
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
    }
	
	private void Update ()
    {
        if (!GameManager.Instance.GetReady) return;

        if(IsGameClear)
        {
            GameManager.Instance.GameClear();
            return;
        }

        Time.timeScale = Parameter.kTimeScale * Parameter.kScriptableTimeScale;
        Parameter.Tick(Time.deltaTime);
        UpdateInput();
        UpdateCharge();
        UpdateVibration();
        CurrentNavigationState.Update(this);
        current_mode_.Update(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        CurrentNavigationState.OnTriggerEnter(this, other);
        current_mode_.OnTriggerEnter(this, other);
    }

    private void OnTriggerExit(Collider other)
    {
        CurrentNavigationState.OnTriggerExit(this, other);
        current_mode_.OnTriggerExit(this, other);
    }

    // 入力
    private void UpdateInput()
    {
        var input = GameManager.Instance.Data.MyInput;
        var leap_motion = LeapMotionInputManager.Instance;
        input_info_.left_punch = input.GetPunchL() | leap_motion.GetPunch(LeapMotionInputManager.HandSide.Left);
        input_info_.right_punch = input.GetPunchR() | leap_motion.GetPunch(LeapMotionInputManager.HandSide.Right);
        input_info_.ultra = input.GetSpecialSkill() | leap_motion.GetUltra();
        input_info_.left_charge = input.GetGyroL().y + (Input.GetKey(KeyCode.LeftArrow) ? -10f : 0f) + leap_motion.GetForearmRotateAcc(LeapMotionInputManager.HandSide.Left);
        input_info_.right_charge = -input.GetGyroR().y + (Input.GetKey(KeyCode.RightArrow) ? 10f : 0f) + leap_motion.GetForearmRotateAcc(LeapMotionInputManager.HandSide.Right);
    }

    // 振動
    private void UpdateVibration()
    {
        var input = GameManager.Instance.Data.MyInput;
        if (vibration_flag_.left_punch_enter)
        {
            vibration_flag_.left_punch_enter = false;
            input.VibrationPunchiShotL();
        }

        if (vibration_flag_.right_punch_enter)
        {
            vibration_flag_.right_punch_enter = false;
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
        bool enable_charge_ = MyAnimator.GetBool("EnableCharge");
        enable_charge_ = UltraController.state == PlayState.Playing ? false : enable_charge_;
        enable_charge_ = IsPlayingEvent == true ? false : enable_charge_;

        ik_controller_.SetActive(enable_charge_);

        float left_amount = 0f;
        float right_amount = 0f;

        if (enable_charge_)
        {
            //ik_controller_.RotateLeft(input_info_.left_charge);
            //ik_controller_.RotateRight(input_info_.right_charge);
            var leap_motion = LeapMotionInputManager.Instance;
            ik_controller_.SetRotationLeft(leap_motion.GetForearmRotate(LeapMotionInputManager.HandSide.Left) + 90f);
            ik_controller_.SetRotationRight(leap_motion.GetForearmRotate(LeapMotionInputManager.HandSide.Right) - 90f);

            left_amount = Mathf.Abs(input_info_.left_charge);
            left_amount = left_amount > kMinChargeAmount ? left_amount : 0f;
            right_amount = Mathf.Abs(input_info_.right_charge);
            right_amount = right_amount > kMinChargeAmount ? right_amount : 0f;
            Parameter.ChangeEnergy(Parameter.ChargeSpeed * Time.deltaTime * (left_amount + right_amount));
        }

        // Charging Effect
        Parameter.LeftHandEffects.chargingEffect.SetPlay(left_amount);
        Parameter.RightHandEffects.chargingEffect.SetPlay(right_amount);
        if((left_amount + right_amount) > 0f)
        {
            SoundManager.Instance.PlaySe("Game_charge000", true);
        }
        else
        {
            SoundManager.Instance.StopLoopSe("Game_charge000", 1f);
        }

        // Energy Effect
        Parameter.LeftHandEffects.chargeEffect.Power = Parameter.CurrentEnergy;
        Parameter.RightHandEffects.chargeEffect.Power = Parameter.CurrentEnergy;
    }
}
