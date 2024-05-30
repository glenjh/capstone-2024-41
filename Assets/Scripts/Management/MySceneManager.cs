using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MySceneManager : MonoBehaviour
{
    public static MySceneManager instance = null;
    
    [Header("Fader")]
    public CanvasGroup fadeImg;
    
    [Header("Loading")]
    public GameObject loadingUI;
    public Slider percentSlider;
    public TextMeshProUGUI loadingText;
    
    [Header("Paused")]
    public GameObject pausedUI;
    public float timer;
    public TextMeshProUGUI timerText;
    public bool countAble;
    
    [Header("Sound")]
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;
    
    [Header("Game Over")] 
    public GameObject overUI;
    private RectTransform overUIRt;
    
    public GameObject nameInput;
    public GameObject rankUI;
    public GameObject ranking;
    public GameObject infoUI;
    private RectTransform rankUIRt;
    public TextMeshProUGUI scoreText;

    [Header("Game Clear")] 
    public GameObject clearUI;
    private RectTransform clearUIRt;
    
    private bool interactAble = true;
    private float duration = 2f;
    
    #region Unity CallBack Func
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(this.gameObject);
        }
        clearUIRt = clearUI.GetComponent<RectTransform>();
        overUIRt = overUI.GetComponent<RectTransform>();
        rankUIRt = rankUI.GetComponent<RectTransform>();
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Main Menu" || !interactAble)
        {
            return;
        }
        
        CountTime();
        EscapeInput();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        fadeImg.DOFade(0, duration).OnStart(() =>
            {
                AudioManager.instance.PlaySFX("TransitionEnd");
                AudioManager.instance.FadeIn(3f);
                countAble = false;
                loadingUI.SetActive(false);
            })
            .OnComplete(() =>
            {
                countAble = true;
                timer = Player.player._playerData.playTime;
                GameManager.instance.isPaused = false;
                interactAble = true;
                fadeImg.blocksRaycasts = false;
            });
    }
    #endregion

    #region SceneChange
    public void ChangeScene(string target)
    {
        Time.timeScale = 1;
        
        fadeImg.DOFade(1, duration)
        .OnStart( () =>
        {
            PoolManager.Instance.ClearPool();
            AudioManager.instance.PlaySFX("TransitionStart");
            AudioManager.instance.FadeOut(3f);
            interactAble = false;
            fadeImg.blocksRaycasts = true;
        })
        .OnComplete( () =>
        {
            StartCoroutine("SceneLoad", target);
        });
    }

    private IEnumerator SceneLoad(string target)
    {
        loadingUI.SetActive(true);
        
        AsyncOperation async = SceneManager.LoadSceneAsync(target);
        async.allowSceneActivation = false;

        float pastTime = 0f;
        float percentage = 0f;

        while (!async.isDone)
        {
            yield return null;

            pastTime += Time.deltaTime;

            if (percentage >= 90f)
            {
                percentage = Mathf.Lerp(percentage, 100, pastTime);
                if (percentage >= 100f)
                {
                    async.allowSceneActivation = true;
                }
            }
            else
            {
                percentage = Mathf.Lerp(percentage, async.progress * 100f, pastTime);
                if (percentage >= 90f)
                {
                    pastTime = 0f;
                }
            }
            
            loadingText.text = percentage.ToString("0") + " %";
            percentSlider.value = percentage;
        }
    }
    #endregion

    #region Pause and Continue
    public void Pause()
    {
        AudioManager.instance.PlaySFX("Paused");
        AudioManager.instance.bgmPlayer.Pause();
        countAble = false;
        Time.timeScale = 0;
        GameManager.instance.isPaused = true;
        pausedUI.SetActive(true);
    }

    public void Continue()
    {
        AudioManager.instance.bgmPlayer.Play();
        countAble = true;
        Time.timeScale = 1;
        GameManager.instance.isPaused = false;
        pausedUI.SetActive(false);
    }

    public void CountTime()
    {
        if (countAble)
        {
            timer += Time.deltaTime;
        }

        int hours = Mathf.FloorToInt(timer / 3600);
        int res = Mathf.FloorToInt(timer % 3600);

        int minutes = Mathf.FloorToInt(res / 60);
        int seconds = Mathf.FloorToInt(res % 60);
        
        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }

    public void EscapeInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!GameManager.instance.isPaused)
            {
                Pause();
            }
            else
            {
                Continue();
            }
        }
    }
    #endregion

    #region Sound
    public void SetMaster()
    {
        float volume = masterSlider.value;
        AudioManager.instance.audioMixer.SetFloat("masterAudio", Mathf.Log10(volume)*20);
    }

    public void SetBGM()
    {
        float volume = bgmSlider.value;
        AudioManager.instance.audioMixer.SetFloat("bgmAudio", Mathf.Log10(volume)*20);
    }

    public void SetSFX()
    {
        float volume = sfxSlider.value;
        AudioManager.instance.audioMixer.SetFloat("sfxAudio", Mathf.Log10(volume)*20);
    }

    public void PlaySFX(string cilp)
    {
        AudioManager.instance.PlaySFX(cilp);
    }
    #endregion

    #region GameOver
    public void GameOver()
    {
        countAble = false;
        interactAble = false;
        fadeImg.DOFade(1, 4f).SetEase(Ease.InOutBounce).SetDelay(0.5f).OnComplete(() =>
        {
            if (DataManager.instance.currMode == GameMode.StoryMode)
            {
                OverUI();
            }
            else
            {
                RankingUI();
            }
        });
    }

    public void OverUI()
    {
        overUI.SetActive(true);
        overUIRt.DOAnchorPosY(0, 2.5f).SetEase(Ease.OutBounce).SetDelay(1f).OnStart((() =>
        {
            AudioManager.instance.PlaySFX("UIDown");
        }));
    }
    
    public void RankingUI()
    {
        rankUI.SetActive(true);
        rankUIRt.DOAnchorPosY(0, 2.5f).SetEase(Ease.OutBounce).SetDelay(1f).OnStart(() =>
        {
            AudioManager.instance.PlaySFX("UIDown");
        }).OnComplete(() =>
        {
            StartCoroutine(Counting(0, DataManager.instance.rushData.score, scoreText));
        });
    }

    public IEnumerator Counting(float start, float end, TextMeshProUGUI word)
    {
        float duration = 0.5f;
        float offser = (end - start) / duration;

        while (start < end)
        {
            AudioManager.instance.PlaySFX("Count");
            start += offser * Time.deltaTime;
            word.text = "Your Score: " + ((int)start).ToString();
            yield return null;
        }

        start = end;
        word.text = "Your Score: " + ((int)start).ToString();
        
        nameInput.SetActive(true);
    }

    public void RankWindowUP()
    {
        rankUIRt.DOAnchorPosY(1120, 1f).SetEase(Ease.InOutBack).OnComplete(() =>
        {
            infoUI.SetActive(true);
            ranking.SetActive(false);
            rankUI.SetActive(false);
        });
    }

    public void OverWindowUP()
    {
        overUIRt.DOAnchorPosY(1120, 1f).SetEase(Ease.InOutBack).OnComplete(() =>
        {
            overUI.SetActive(false);
        });
    }
    #endregion

    #region GameClear

    public void GameClear()
    {
        GameManager.instance.isClear = true;
        countAble = false;
        interactAble = false;
        if (DataManager.instance.currMode == GameMode.StoryMode)
        {
            ClearUI();
        }
        else
        {
            RankingUI();
        }
    }
    
    public void ClearUI()
    {
        clearUI.SetActive(true);
        clearUIRt.DOAnchorPosY(0, 1f).SetEase(Ease.OutBounce).SetDelay(1.5f).OnStart((() =>
        {
            AudioManager.instance.PlaySFX("UIDown");
        }));
    }

    public void ClearWindowUP()
    {
        clearUIRt.DOAnchorPosY(1120, 1f).SetEase(Ease.InOutBack).OnComplete(() =>
        {
            clearUI.SetActive(false);
        });
    }
    public void ClearOrOver()
    {
        if (GameManager.instance.isClear)
        {
            ClearUI();
            GameManager.instance.isClear = false;
        }
        else
        {
            OverUI();
        }
    }
    #endregion
    
    public string GetCurrentScene()
    {
        return SceneManager.GetActiveScene().name;
    }
    
    public void Restart()
    {
        Player.player._playerData.life = 10;
        Player.player._playerData.charge = 0f;
        ChangeScene(GetCurrentScene());
    }

    #region About Data
    public void DataSava()
    {
        DataManager.instance.SaveData();
    }

    public void DataInit()
    {
        DataManager.instance.InitData();
    }

    public void DataClear()
    {
        DataManager.instance.ClearData();
    }
    #endregion
}
