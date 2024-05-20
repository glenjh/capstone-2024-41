using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCheck : MonoBehaviour
{
    public Player player;
    public int damage;

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Enemy") || col.gameObject.CompareTag("Boss"))
        {
            Debug.Log("Hit");
            if (!player.isRage)
            {
                player.Charge();
            }
            col.GetComponent<IDamageAble>()?.TakeHit(damage,this.transform);
            CameraManager.instance.CamShake();
            player._pauser.HitStop(0.1f); 
        }
    }
}
