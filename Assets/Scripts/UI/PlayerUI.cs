using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerUI : MonoBehaviour
{
    [Header("Player")]
    public Player player;
    
    [Header("UI")]
    public GameObject statusUIGroup;
    public RectTransform statusRT;

    public GameObject skillUIGroup;
    public RectTransform skillRT;

    public GameObject upFader;
    public RectTransform upFaderRT;
    
    public GameObject downFader;
    public RectTransform downFaderRT;
    
    [Header("HP")]
    public Slider hpBar;
    public Slider chargeBar;
    
    [Header("Charge")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI chargeText;

    [Header("Q Skill")] 
    public GameObject qSkill;
    public Image qImage;
    
    [Header("W Skill")] 
    public GameObject wSkill;
    public Image wImage;
    
    [Header("E Skill")] 
    public GameObject eSkill;
    public Image eImage;
    
    [Header("R Skill")] 
    public GameObject rSkill;
    public Image rImage;

    public void Start()
    {
        Init();
        statusRT = statusUIGroup.GetComponent<RectTransform>();
        skillRT = skillUIGroup.GetComponent<RectTransform>();
        upFaderRT = upFader.GetComponent<RectTransform>();
        downFaderRT = downFader.GetComponent<RectTransform>();
    }

    public void Init()
    {
        SetInitHealth();
        SetInitCharge();
    }

    public void SetInitHealth()
    {
        hpBar.maxValue = player.maxLife;
        hpBar.value = player.life;
    }

    public void SetInitCharge()
    {
        chargeBar.maxValue = 100;
        chargeBar.value = player.currCharge;
    }

    public void LateUpdate()
    {
        //HP
        hpText.text = player.life + " / " + player.maxLife;
        float hp = Mathf.Lerp(hpBar.value, player.life, Time.deltaTime * 10);
        hpBar.value = hp;
        
        //Charge
        chargeText.text = player.currCharge + " / " + player.maxCharge;
        float charge = Mathf.Lerp(chargeBar.value, player.currCharge, Time.deltaTime * 10);
        chargeBar.value = charge;
        
        //Q Skill
        if (player.pulseUnlocked)
        {
            qSkill.SetActive(true);
        }
        
        //W Skill
        if (player.wUnlocked)
        {
            wSkill.SetActive(true);
        }
        
        //E Skill
        if (player.eUnlocked)
        {
            eSkill.SetActive(true);
        }
        
        //R Skill
        if (player.rUnlocked)
        {
            rSkill.SetActive(true);
        }
    }

    public void HideSignal()
    {
        //Hide UIs
        statusRT.DOAnchorPosY(-230, 1.5f).SetEase(Ease.OutQuart);
        skillRT.DOAnchorPosX(-180, 1.5f).SetEase(Ease.OutQuart);
        
        //Show Faders
        upFaderRT.DOAnchorPosY(0, 1.5f).SetEase(Ease.OutQuart);
        downFaderRT.DOAnchorPosY(0, 1.5f).SetEase(Ease.OutQuart);
    }

    public void ShowSignal()
    {
        //Show UIs
        statusRT.DOAnchorPosY(0, 1.5f).SetEase(Ease.OutQuart);
        skillRT.DOAnchorPosX(-0, 1.5f).SetEase(Ease.OutQuart);
        
        //Hide Faders
        upFaderRT.DOAnchorPosY(120, 1.5f).SetEase(Ease.OutQuart);
        downFaderRT.DOAnchorPosY(-120, 1.5f).SetEase(Ease.OutQuart);
    }

    public IEnumerator SkillDown(Image fillImage, float cool)
    {
        float temp = 0f;
        while (temp <= cool)
        {
            temp += Time.deltaTime;
            fillImage.fillAmount = Mathf.Lerp(0, 1,temp / cool);
            yield return new WaitForFixedUpdate();
        }
    }
}
