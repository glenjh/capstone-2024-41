using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseAttack : MonoBehaviour
{
    [SerializeField] private Collider2D pulseCollider;

    public CircleCollider2D range;

    public void Start()
    {
        range = GetComponent<CircleCollider2D>();
    }
    
    public void PulseOn()
    {
        range.enabled = true;
    }

    public void PulseOff()
    {
        range.enabled = false;
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            col.GetComponent<IDamageAble>()?.TakeHit(1,null);
        }
    }
}
