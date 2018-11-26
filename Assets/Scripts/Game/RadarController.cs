using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarController : MonoBehaviour
{
    [SerializeField] float kRotateSpeed = 60f;
    [SerializeField] float kBgmWaitTIme = 1f;
    private Vector3 rotation_ = Vector3.zero;
    private float wait_time_ = 0f;

    private void Start()
    {
        rotation_ = transform.localRotation.eulerAngles;
    }

    // Update is called once per frame
    private void Update ()
    {
        rotation_.y += kRotateSpeed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(rotation_);

        if(wait_time_ > 0f)
        {
            wait_time_ -= Time.unscaledDeltaTime;
            if(wait_time_ <= 0f)
            {
                SoundManager.Instance.PlayBgm("Result_BGM000");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (other.gameObject.name.Equals("UltraCollider"))
            {
                GameManager.Instance.Data.Player.OnUltraHit();
                wait_time_ = kBgmWaitTIme;
                GameManager.Instance.Data.MyInput.SetLedLightUp();
            }
        }
    }
}
