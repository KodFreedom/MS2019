using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private struct LoopSe
    {
        public float stop_time;
        public AudioSource audio_source;
    }

    [SerializeField] float BgmFadeTime = 0.5f;
    [SerializeField] AudioSource BgmPlayer = null;
    [SerializeField] AudioSource SePlayer = null;
    [SerializeField, Range(0f, 1f)] float MaxBgmVolume = 0.5f;
    [SerializeField] AudioClip[] SoundEffects = new AudioClip[0];
    [SerializeField] AudioClip[] BackgroundMusics = new AudioClip[0];
    private Dictionary<string, int> sound_effects_ = new Dictionary<string, int>();
    private Dictionary<string, int> background_musics_ = new Dictionary<string, int>();
    private Dictionary<string, LoopSe> looping_ses_ = new Dictionary<string, LoopSe>();
    private int current_bgm_ = -1;
    private int next_bgm_ = -1;
    public static SoundManager Instance { get; private set; }

    public void PlaySe(string name, bool loop)
    {
        //if (!sound_effects_.ContainsKey(name)) return;

        //if (SoundEffects[sound_effects_[name]].loadState == AudioDataLoadState.Loaded)
        //{
        //    if (!loop)
        //    {
        //        SePlayer.PlayOneShot(SoundEffects[sound_effects_[name]]);
        //    }
        //    else
        //    {
        //        if(looping_ses_.ContainsKey(name))
        //        {
        //            //var loop_se = looping_ses_[name];
        //            //loop_se.stop_time = -1f;
        //            //loop_se.audio_source.volume = 1f;
        //            //looping_ses_[name] = loop_se;
        //        }
        //        else
        //        {
        //            LoopSe loop_se = new LoopSe();
        //            loop_se.stop_time = -1f;
        //            loop_se.audio_source = SePlayer.gameObject.AddComponent<AudioSource>();
        //            loop_se.audio_source.clip = SoundEffects[sound_effects_[name]];
        //            loop_se.audio_source.loop = true;
        //            loop_se.audio_source.Play();
        //            looping_ses_.Add(name, loop_se);
        //        }
        //    }
        //}
    }

    public void StopLoopSe(string name, float stop_time)
    {
        //if (!looping_ses_.ContainsKey(name)) return;
        //var loop_se = looping_ses_[name];
        //loop_se.stop_time = stop_time;
        //looping_ses_[name] = loop_se;
    }

    public void PlayBgm(string name)
    {
        //if (!background_musics_.ContainsKey(name)) return;
        //Debug.Log("PlayBgm : " + name);
        //next_bgm_ = background_musics_[name];
    }

    public void StopBgm(string name)
    {
        //if (!background_musics_.ContainsKey(name)) return;
        //if(current_bgm_ == background_musics_[name])
        //{
        //    current_bgm_ = -1;
        //}
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
            PlaySe("bilibili-long", true);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            StopLoopSe("bilibili-long", 1f);
        }
        UpdateSe();
        UpdateBgm();
    }

    private void CreateDictionary(AudioClip[] sources, Dictionary<string, int> dictionary)
    {
        for(int count = 0; count < sources.Length; ++count)
        {
            dictionary.Add(sources[count].name, count);
        }
    }

    private void UpdateSe()
    {
        foreach(var se in looping_ses_)
        {
            var loop_se = se.Value;
            if(loop_se.stop_time >= 0f)
            {
                loop_se.audio_source.volume = loop_se.stop_time == 0f ? 0f :
                    loop_se.audio_source.volume - Time.unscaledDeltaTime / loop_se.stop_time;
            }

            if(loop_se.audio_source.volume <= 0f)
            {
                loop_se.audio_source.Stop();
            }

            if(!loop_se.audio_source.isPlaying)
            {
                looping_ses_.Remove(se.Key);
                Destroy(loop_se.audio_source);
                if (looping_ses_.Count <= 0) break;
            }
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
            BgmPlayer.volume = Mathf.Min(BgmPlayer.volume + Time.unscaledDeltaTime / BgmFadeTime * MaxBgmVolume, MaxBgmVolume);
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
        BgmPlayer.volume -= Time.unscaledDeltaTime / BgmFadeTime * MaxBgmVolume;
        if (BgmPlayer.volume <= 0f)
        {
            BgmPlayer.Stop();
        }
    }
}
