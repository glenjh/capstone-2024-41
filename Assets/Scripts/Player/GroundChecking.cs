using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
            _player.isGround = true;
            _player.anim.SetBool("isFalling", false);
            _player.anim.SetBool("isJumping", false);
        }
    }

    public void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "Ground")
        {
            if (_player.PS == PlayerStates.Stamping)
            {
                _player.StampAttack();
            }
            
            _player.PS = PlayerStates.Idle;
            _player.HandleInput();
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Ground")
        {
            _player.isGround = false;
        }
    }
}
