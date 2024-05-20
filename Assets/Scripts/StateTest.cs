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
            // DataManager.instance.storyData.additionalStates[0] = true;
            player._playerData.additionalStates[0] = true;
            player._stateMachine.AddState(PlayerStates.PARRYING);
            
            // DataManager.instance.storyData.additionalStates[1] = true;
            player._playerData.additionalStates[1] = true;
            player._stateMachine.AddState(PlayerStates.WALLSLIDING);
            player._stateMachine.AddState(PlayerStates.WALLJUMPING);
            
            Destroy(this.gameObject);
        }
    }
}
