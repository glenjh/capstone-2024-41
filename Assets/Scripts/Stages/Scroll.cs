using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroll : MonoBehaviour
{
    public Material bgMaterial;
    public float scrollSpeed = 0.2f;

    void Update()
    {
        Vector2 direction = Vector2.right;
        bgMaterial.mainTextureOffset += direction * scrollSpeed * Time.deltaTime;
    }
}
