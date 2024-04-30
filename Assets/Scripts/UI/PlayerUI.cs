using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerUI : MonoBehaviour
{
    [Header("Player UI")]
    public Player player;
    public GameObject uiGroup;
    public RectTransform rt;
    
    [Header("HP")]
    public Slider hpBar;
    public Slider chargeBar;
    
    [Header("Charge")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI chargeText;

    public void Start()
    {
        Init();
        rt = uiGroup.GetComponent<RectTransform>();
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
        chargeBar.value = 0;
    }

    public void LateUpdate()
    {
        hpText.text = player.life + " / " + player.maxLife;
        float hp = Mathf.Lerp(hpBar.value, player.life, Time.deltaTime * 10);
        hpBar.value = hp;
        
        chargeText.text = player.currCharge + " / " + player.maxCharge;
        float charge = Mathf.Lerp(chargeBar.value, player.currCharge, Time.deltaTime * 10);
        chargeBar.value = charge;
    }

    public void HideSignal()
    {
        rt.DOAnchorPosY(-230, 1.5f).SetEase(Ease.OutQuart);
    }

    public void ShowSignal()
    {
        rt.DOAnchorPosY(0, 1.5f).SetEase(Ease.OutQuart);
    }
}
