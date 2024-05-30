using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    Sequence startSeq;

    public CanvasGroup mainGroup;
    public TextMeshProUGUI mainTitle;
    public GameObject startText;
    public Image mainBorder;
    public GameObject playOrQuit;

    private bool interactable = false; // ignore User's Input
    
    
    void Start()
    {
        startSeq = DOTween.Sequence().SetAutoKill(false)
            .Append(mainTitle.DOColor(Color.white, 3f)).Join(mainBorder.DOColor(Color.white, 2))
            .OnStart(() =>
            {
                interactable = false;
                mainGroup.blocksRaycasts = false;
            })
            .OnComplete(() =>
            {
                interactable = true;
                mainGroup.blocksRaycasts = true;
                startText.SetActive(true);
                startSeq.Kill();
            });
        
        startSeq.Play();
    }

    public void Update()
    {
        if (Input.anyKeyDown && interactable)
        {
            interactable = false;
            startText.SetActive(false);
            playOrQuit.SetActive(true);
        }
    }
    
    public void SetStoryMode()
    {
        DataManager.instance.currMode = GameMode.StoryMode;
    }

    public void SetBossRushMode()
    {
        DataManager.instance.currMode = GameMode.BossRushMode;
    }
    
    public void StartStoryMode()
    {
        DataManager.instance.LoadStoryData();
    }
    
    public void StartBossRushMode()
    {
        DataManager.instance.LoadRushData();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlaySFX()
    {
        AudioManager.instance.PlaySFX("MouseClick");
    }
}
