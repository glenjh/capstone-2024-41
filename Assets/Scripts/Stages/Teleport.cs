using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Player player;
    public BoxCollider2D col;
    public Animator anim;
    public GameObject buttonImg;
    public string nextStage;

    public void Start()
    {
        col = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    public void Update()
    {
        if (col.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            buttonImg.SetActive(true);
            if (Input.GetKeyDown(KeyCode.F))
            {
                player.GetComponent<SpriteRenderer>().enabled = false;
                col.enabled = false;
                anim.SetBool("isWarp", true);

                StartCoroutine("TonextStage");
            }
        }
        else
        {
            buttonImg.SetActive(false);
        }
    }

    public IEnumerator TonextStage()
    {
        if (DataManager.instance.currMode == GameMode.BossRushMode)
        {
            DataManager.instance.rushData.score += 1000;
        }
        player.rigid.velocity = new Vector2(0, player.rigid.velocity.y);
        player.controlAble = false;
        
        yield return new WaitForSecondsRealtime(1f);
        
        MySceneManager.instance.ChangeScene(nextStage);
    }
}
