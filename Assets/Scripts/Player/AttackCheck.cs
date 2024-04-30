using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCheck : MonoBehaviour
{
    public Player player;

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Enemy") || col.gameObject.CompareTag("Boss"))
        {
            if (!player.isRage)
            {
                player.Charge();
            }
            col.GetComponent<IDamageAble>()?.TakeHit(1,null);
            CameraManager.instance.CamShake();
        }
    }
}
