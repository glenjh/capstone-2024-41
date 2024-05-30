using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UpgradeChip : MonoBehaviour
{
    public GameObject chipUI;
    public BoxCollider2D col;
    public string bossType;

    void Start()
    {
        col = GetComponent<BoxCollider2D>();
        this.gameObject.SetActive(false);
    }

    public void Update()
    {
        if (col.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            chipUI.transform.DOScale(1f, 0.7f).SetEase(Ease.OutQuart);
            if (Input.GetKeyDown(KeyCode.F))
            {
                AchiveManager.BossDefeated(bossType);
                AudioManager.instance.PlaySFX("Upgrade");
                chipUI.SetActive(false);
                this.gameObject.SetActive(false);
            }
        }
        else
        {
            chipUI.transform.DOScale(0f, 0.7f).SetEase(Ease.OutQuart);
        }
    }
    
    public void ActiveSignal()
    {
        AudioManager.instance.PlaySFX("Chip");
        this.gameObject.SetActive(true);
    }
}
