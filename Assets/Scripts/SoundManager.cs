using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] float BgmFadeTime = 0.5f;
    [SerializeField] AudioSource BgmPlayer = null;
    [SerializeField] AudioSource SePlayer = null;
    [SerializeField] AudioClip[] SoundEffects = new AudioClip[0];
    [SerializeField] AudioClip[] BackgroundMusics = new AudioClip[0];
    private Dictionary<string, int> sound_effects_ = new Dictionary<string, int>();
    private Dictionary<string, int> background_musics_ = new Dictionary<string, int>();
    private int current_bgm_ = -1;
    private int next_bgm_ = -1;
    public static SoundManager Instance { get; private set; }

    public void PlaySe(string name)
    {
        if (!sound_effects_.ContainsKey(name)) return;
        if(SoundEffects[sound_effects_[name]].loadState == AudioDataLoadState.Loaded)
        {
            Debug.Log("PlaySe : " + name);
            SePlayer.PlayOneShot(SoundEffects[sound_effects_[name]]);
        }
    }

    public void PlayBgm(string name)
    {
        if (!background_musics_.ContainsKey(name)) return;
        Debug.Log("PlayBgm : " + name);
        next_bgm_ = background_musics_[name];
    }

    public void StopBgm(string name)
    {
        if (!background_musics_.ContainsKey(name)) return;
        if(current_bgm_ == background_musics_[name])
        {
            current_bgm_ = -1;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        CreateDictionary(SoundEffects, sound_effects_);
        CreateDictionary(BackgroundMusics, background_musics_);
    }

    private void LateUpdate()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            PlayBgm("the truth that you leave me");
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            PlaySe("bilibili-long");
        }
        UpdateBgm();
    }

    private void CreateDictionary(AudioClip[] sources, Dictionary<string, int> dictionary)
    {
        for(int count = 0; count < sources.Length; ++count)
        {
            dictionary.Add(sources[count].name, count);
        }
    }

    private void UpdateBgm()
    {
        if(next_bgm_ != -1)
        {
            if(BgmPlayer.isPlaying)
            {
                StopBgm();
            }
            else
            {
                current_bgm_ = next_bgm_;
                next_bgm_ = -1;
                BgmPlayer.clip = BackgroundMusics[current_bgm_];
                BgmPlayer.Play();
            }

            return;
        }

        if(current_bgm_ != -1)
        {
            BgmPlayer.volume += Time.unscaledDeltaTime / BgmFadeTime;
        }
        else
        {
            if (BgmPlayer.isPlaying)
            {
                StopBgm();
            }
        }
    }

    private void StopBgm()
    {
        BgmPlayer.volume -= Time.unscaledDeltaTime / BgmFadeTime;
        if (BgmPlayer.volume <= 0f)
        {
            BgmPlayer.Stop();
        }
    }
}
