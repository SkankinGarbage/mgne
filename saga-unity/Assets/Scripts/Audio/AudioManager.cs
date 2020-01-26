using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    private const string NoBGMKey = "none";
    private const float FadeSeconds = 0.5f;

    private AudioSource sfxSource;
    private AudioSource bgmSource;

    private float baseVolume = 1.0f;
    private Setting<float> bgmVolumeSetting;
    private Setting<float> sfxVolumeSetting;

    public string CurrentBGMKey { get; private set; }

    public void Awake() {
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.playOnAwake = false;
        bgmSource.loop = true;

        CurrentBGMKey = NoBGMKey;
        
        sfxVolumeSetting = Global.Instance().Settings.GetFloatSetting(SettingsConstants.SoundEffectVolume);
        bgmVolumeSetting = Global.Instance().Settings.GetFloatSetting(SettingsConstants.BGMVolume);
        
        gameObject.AddComponent<AudioListener>();
    }

    public void Update() {
        bgmSource.volume = bgmVolumeSetting.Value * baseVolume;
        sfxSource.volume = sfxVolumeSetting.Value * baseVolume;
    }

    public void PlaySFX(Enum enumValue) {
        PlaySFX(enumValue.ToString());
    }
    public void PlaySFX(string key) {
        AudioClip clip = IndexDatabase.Instance().SFX.GetData(key).clip;
        StartCoroutine(PlaySFXRoutine(sfxSource, clip));
    }

    public void PlayBGM(string key) {
        if (key != CurrentBGMKey) {
            CurrentBGMKey = key;
            if (key == null || key == NoBGMKey) {
                bgmSource.Stop();
            } else {
                bgmSource.volume = 1.0f;
                AudioClip clip = IndexDatabase.Instance().BGM.GetData(key).track;
                bgmSource.clip = clip;
                bgmSource.Play();
            }
        }
    }

    public AudioClip BGMClip() {
        return bgmSource.clip;
    }

    public IEnumerator FadeOutRoutine(float durationSeconds) {
        CurrentBGMKey = NoBGMKey;
        while (baseVolume > 0.0f) {
            baseVolume -= Time.deltaTime / durationSeconds;
            if (baseVolume < 0.0f) {
                baseVolume = 0.0f;
            }
            yield return null;
        }
        baseVolume = 1.0f;
        PlayBGM(NoBGMKey);
    }

    public IEnumerator CrossfadeRoutine(string tag) {
        if (CurrentBGMKey != null && CurrentBGMKey != NoBGMKey) {
            yield return FadeOutRoutine(FadeSeconds);
        }
        PlayBGM(tag);
    }

    private IEnumerator PlaySFXRoutine(AudioSource source, AudioClip clip) {
        while (clip.loadState == AudioDataLoadState.Loading) {
            yield return null;
        }
        if (clip.loadState == AudioDataLoadState.Loaded) {
            source.clip = clip;
            source.Play();
        }
    }
}
