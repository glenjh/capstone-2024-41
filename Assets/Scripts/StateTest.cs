using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTest : MonoBehaviour
{
    public Player player;
    public BoxCollider2D col;
    
    void Start()
    {
        col = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player._stateMachine.AddState(PlayerStates.PARRYING);
            player._stateMachine.AddState(PlayerStates.WALLSLIDING);
            player._stateMachine.AddState(PlayerStates.WALLJUMPING);
            
            Destroy(this.gameObject);
        }
    }
}
