using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunder : MonoBehaviour {
    BoxCollider2D _collider;
    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
    }

    public void OffThunder()
    {
        _collider.enabled = false;
        gameObject.SetActive(false);
    }

    public void OnCollider()
    {
        _collider.enabled = true;
    }
}