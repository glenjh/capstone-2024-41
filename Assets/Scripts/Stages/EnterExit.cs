using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnterExit : MonoBehaviour
{
    [SerializeField] public GameObject offTilemap;
    [SerializeField] public GameObject onTilemap;
    public bool isActive = true;

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            onTilemap.SetActive(isActive);
            isActive = !isActive;
            offTilemap.SetActive(isActive);
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            onTilemap.SetActive(isActive);
            isActive = !isActive;
            offTilemap.SetActive(isActive);
        }
    }
}
