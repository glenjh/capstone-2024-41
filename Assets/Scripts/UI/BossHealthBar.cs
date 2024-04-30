using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BossHealthBar : MonoBehaviour
{
    public Slider healthBar;
    public Boss boss;
    private RectTransform rt;

    public void Start()
    {
        rt = GetComponent<RectTransform>();
        SetMaxHealth();
    }

    public void SetMaxHealth()
    {
        healthBar.maxValue = boss.maxHealth;
        healthBar.value = boss.maxHealth;
    }

    public void SetHealth(float health)
    {
        healthBar.value = health;
    }

    public void LateUpdate()
    {
        float hp = Mathf.Lerp(healthBar.value, boss.health, Time.deltaTime * 10);
        healthBar.value = hp;
    }

    public void ShowSignal()
    {
        rt.DOAnchorPosY(0, 1.5f).SetDelay(0.5f).SetEase(Ease.OutBounce);
    }

    public void HideSignal()
    {
        rt.DOAnchorPosY(150, 1.5f).SetDelay(1.5f).SetEase(Ease.OutQuart);
    }
}
