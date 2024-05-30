using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.UI;

public class TimelinePlayer : MonoBehaviour
{
    [SerializeField] 
    private PlayableDirector EnterTimeLine;
    public AudioClip bossBGM;
    public BoxCollider2D col;
    public Player player;
    public Image flash;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player.rigid.velocity = new Vector2(0, player.rigid.velocity.y);
            player.rigid.gravityScale = player.normalGravity;
            player._stateMachine.ChangeState(PlayerStates.IDLE);
            player.controlAble = false;
            col.enabled = false;
            AudioManager.instance.FadeOut(0.2f);
            StartCoroutine("PlayBuffering");
        }
    }

    IEnumerator PlayBuffering()
    {
        yield return new WaitForSeconds(0.5f);
        AudioManager.instance.bgmPlayer.clip = bossBGM;
        AudioManager.instance.FadeIn(7f);
        EnterTimeLine.Play();
    }

    public void Flash()
    {
        AudioManager.instance.PlaySFX("BossDie");
        AudioManager.instance.bgmPlayer.Pause();
        player.rigid.velocity = new Vector2(0, player.rigid.velocity.y);
        player.rigid.gravityScale = player.normalGravity;
        
        flash.DOFade(0.9f, 0.3f).OnStart(() =>
        {
            Time.timeScale = 0.5f;
        }).OnComplete(() =>
        {
            player.rigid.velocity = new Vector2(0, player.rigid.velocity.y);
            player._stateMachine.ChangeState(PlayerStates.IDLE);
            flash.DOFade(0, 1f).SetDelay(0.5f).OnComplete(() =>
            {
                Time.timeScale = 1;
            });
        });
    }
}
