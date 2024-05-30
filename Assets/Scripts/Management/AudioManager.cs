using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

[Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null;

    public AudioMixer audioMixer;
    public AudioSource bgmPlayer;
    public AudioSource[] sfxPlayer;
    
    public Sound[] BGM;
    public Sound[] SFX;
    
    # region Singleton
    void Init()
    {
        if (instance == null)
        {
            instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            DontDestroyOnLoad(this);
        }

        else
        {
            Destroy(this.gameObject);
        }
    }

    void Awake()
    {
        Init();
    }
    # endregion

    #region BGM
    public void PlayBGM(string bgmName)
    {
        StopBGM();
        for (int i = 0; i < BGM.Length; i++)
        {
            if (BGM[i].name == bgmName)
            {
                bgmPlayer.clip = BGM[i].clip;
                bgmPlayer.volume = 0.4f;
                bgmPlayer.Play();
            }
        }
    }

    public void StopBGM()
    {
        bgmPlayer.Stop();
    }

    public void FadeOut(float duration)
    {
        StartCoroutine(FadeOutCoroutine(duration));
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float startVolume = bgmPlayer.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            bgmPlayer.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        bgmPlayer.volume = 0f;
        bgmPlayer.Stop();
    }
    
    public void FadeIn(float duration)
    {
        StartCoroutine(FadeInCoroutine(duration));
    }

    private IEnumerator FadeInCoroutine(float duration)
    {
        float startVolume = 0f;
        float endVolum = 0.4f;
        bgmPlayer.volume = startVolume;
        bgmPlayer.Play();

        float elapsed = 0f;
        while (elapsed < duration)
        {
            bgmPlayer.volume = Mathf.Lerp(startVolume, endVolum, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        bgmPlayer.volume = endVolum;
    }

    public void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        for (int i = 0; i < BGM.Length; i++)
        {
            if (BGM[i].name == arg0.name)
            {
                PlayBGM(BGM[i].name);
            }
        }
    }
    #endregion

    #region SFX
    public void PlaySFX(string sfxName)
    {
        for (int i = 0; i < SFX.Length; i++)
        {
            if (SFX[i].name == sfxName)
            {
                for (int j = 0; j < sfxPlayer.Length; j++)
                {
                    if (!sfxPlayer[j].isPlaying)
                    {
                        sfxPlayer[j].volume = 0.5f;
                        sfxPlayer[j].clip = SFX[i].clip;
                        sfxPlayer[j].Play();
                        return;
                    }
                }
            }
        }
    }

    public void DeadSound()
    {
        bgmPlayer.Pause();
        StartCoroutine("Delay");
        PlaySFX("Die");
    }

    public IEnumerator Delay()
    {
        yield return new WaitForSecondsRealtime(0.2f);
    }
    #endregion
}
