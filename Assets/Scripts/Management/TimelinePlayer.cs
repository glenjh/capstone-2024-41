using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelinePlayer : MonoBehaviour
{
    [SerializeField] 
    private PlayableDirector timeLine;
    public BoxCollider2D col;
    public Player player;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player.rigid.velocity = new Vector2(0, player.rigid.velocity.y);
            player.rigid.gravityScale = player.normalGravity;
            player._stateMachine.ChangeState(PlayerStates.IDLE);
            player.controlAble = false;
            col.enabled = false;
            StartCoroutine("PlayBuffering");
        }
    }

    IEnumerator PlayBuffering()
    {
        yield return new WaitForSeconds(0.5f);
        timeLine.Play();
    }
}
