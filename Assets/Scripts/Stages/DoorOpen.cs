using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class DoorOpen : MonoBehaviour
{
    public Player player;
    public BoxCollider2D col;
    public Animator anim;
    public GameObject buttonImg;
    
    void Start()
    {
        player = FindObjectOfType<Player>();
        col = GetComponent<BoxCollider2D>();
        anim = GetComponentInParent<Animator>();
    }

    public void Update()
    {
        if (col.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            buttonImg.SetActive(true);
            if (Input.GetKeyDown(KeyCode.F))
            {
                AudioManager.instance.PlaySFX("DoorOpen");
                col.enabled = false;
                anim.SetBool("isOpening", true);
            }
        }
        else
        {
            buttonImg.SetActive(false);
        }
    }
}
