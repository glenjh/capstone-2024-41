using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTest : MonoBehaviour
{
    public BoxCollider2D boxCol;
    public Animator anim;
    
    void Start()
    {
        boxCol = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    public void Open()
    {
        anim.SetBool("isOpened", true);
    }

    public void Close()
    {
        anim.SetBool("isOpened", false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Open();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Close();
        }
    }
}
