using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingTrap : MonoBehaviour
{
    [SerializeField] private BoxCollider2D hitBoxCol;
    private Player _player;

    void Start()
    {
        _player = FindObjectOfType<Player>();
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player") && !_player.isHit)
        {
            _player.TakeHit(1, null);
        }
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")&& !_player.isHit)
        {
            _player.TakeHit(1, null);
        }
    }
}
