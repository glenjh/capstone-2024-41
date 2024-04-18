using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parrying : MonoBehaviour
{
    public Player _player;

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("EnemyBullet"))
        {
            _player.StartCoroutine(_player.OnDamage(1f));
            _player.ParryingSuccess();
        }
    }
}
