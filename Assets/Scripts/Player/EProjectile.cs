using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EProjectile : MonoBehaviour
{
    private Rigidbody2D rigid;
    
    private float lifeTime;

    private void Awake()
    {
        lifeTime = 2f;
        rigid = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            return;
        }
        else if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Boss"))
        {
            other.GetComponent<IDamageAble>()?.TakeHit(2,this.transform);
            var player = FindObjectOfType<Player>();
            player.transform.position = other.transform.position;
        }
        Destroy(this.gameObject);
    }
}
