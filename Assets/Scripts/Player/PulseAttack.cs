using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseAttack : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            col.GetComponent<IDamageAble>()?.TakeHit(1,this.transform);
        }
    }
}
