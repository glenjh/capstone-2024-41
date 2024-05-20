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
                bgmPlayer.Play();
            }
        }
    }

    public void StopBGM()
    {
        bgmPlayer.Stop();
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
                        sfxPlayer[j].clip = SFX[i].clip;
                        sfxPlayer[j].Play();
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else
            {
                return;
            }
        }
    }
    #endregion
}
