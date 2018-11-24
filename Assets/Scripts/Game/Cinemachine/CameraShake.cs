using UnityEngine;
using Cinemachine;

/// <summary>
/// An add-on module for Cinemachine to shake the camera
/// </summary>
[ExecuteInEditMode]
[SaveDuringPlay]
[AddComponentMenu("")] // Hide in menu
public class CameraShake : CinemachineExtension
{
    [Tooltip("Amplitude of the shake")]
    private float max_range_ = 0f;
    private float max_time_ = 0f;
    private float timer_ = 0f;

    public void Shake(float max_range, float max_time)
    {
        if (max_time_ != 0f) return;
        SetParameter(max_range, max_time, 0f);
    }

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Body)
        {
            if(max_time_ > 0f)
            {
                float rate = (max_time_ - timer_) / max_time_;
                Vector3 shake_amount = GetOffset() * rate;
                state.PositionCorrection += shake_amount;
                timer_ += Time.deltaTime;
                if(timer_ > max_time_)
                {
                    SetParameter(0f, 0f, 0f);
                }
            }

        }
    }

    Vector3 GetOffset()
    {
        // Note: change this to something more interesting!
        return new Vector3(
            Random.Range(-max_range_, max_range_),
            Random.Range(0f, max_range_),
            0f);
    }

    private void SetParameter(float max_range, float max_time, float timer)
    {
        max_time_ = max_time;
        max_range_ = max_range;
        timer_ = timer;
    }
}