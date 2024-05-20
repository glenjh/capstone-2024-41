using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentBG : MonoBehaviour
{
    public SpriteRenderer sprite;
    private Color originColor;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            originColor = sprite.color;
            sprite.color = new Color(originColor.r, originColor.g, originColor.b, 0.8f);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            sprite.color = originColor;
        }
    }
}
