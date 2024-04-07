using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCheck : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Enemy") || col.gameObject.CompareTag("Boss"))
        {
            col.GetComponent<IDamageAble>()?.TakeHit(1,null);
        }
    }
}
