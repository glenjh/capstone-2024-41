using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    Rigidbody2D _rigidbody2D;
    public bool activeSelf;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }
    
    private void OnEnable()
    {
        StartCoroutine(DisableBullet());
    }
    
    IEnumerator DisableBullet()
    {
        yield return new WaitForSeconds(1.5f);
    }
    
    public void Fire(Vector3 dir, float speed)
    {
        //addforce 방식
        _rigidbody2D.AddForce(dir * speed, ForceMode2D.Impulse);
    }
    
    private void OnDisable()
    {
        StopAllCoroutines();
    }
    private void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.SetActive(false);
        }
    }
}