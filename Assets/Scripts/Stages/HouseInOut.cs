using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseInOut : MonoBehaviour
{
    public GameObject houseInObjs;
    public GameObject houseOutObjs;
    public BoxCollider2D col;
    
    void Start()
    {
        col = GetComponent<BoxCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            houseOutObjs.SetActive(false);
            houseInObjs.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            houseOutObjs.SetActive(true);
            houseInObjs.SetActive(false);
        }
    }
}
