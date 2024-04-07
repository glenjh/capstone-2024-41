using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    public string bossType = "Dummy";
    public int HP = 3;

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            if (HP >= 1)
            {
                HP -= 1;
            }
            else
            {
                AchiveManager.BossDefeated(bossType);
                GameObject.Destroy(gameObject);
            }
        }
    }
}
