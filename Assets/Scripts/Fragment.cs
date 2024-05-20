using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fragment : MonoBehaviour
{
    Rigidbody2D rb;

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void Init(Transform startPos, Transform scale, Vector2 direction, 
        float speed, float lifeTime, bool isRotate, float rotateSpeed)
    {
        transform.position = startPos.position;
        transform.localScale = scale.localScale;
        rb.velocity = direction * speed;
        Destroy(gameObject, lifeTime);
        if (isRotate)
        {
            rb.angularVelocity = rotateSpeed;
        }
    }
}
