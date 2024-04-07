using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecking : MonoBehaviour
{
    private Player _player;
    void Start()
    {
        _player = transform.parent.GetComponent<Player>();
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Ground")
        {
            _player.PS = PlayerStates.Idle;
            _player.HandleInput();
        }
    }
}
