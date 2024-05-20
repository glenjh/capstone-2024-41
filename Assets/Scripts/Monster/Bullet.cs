using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    Rigidbody2D _rigidbody2D;
    Collider2D _collider2D;
    public bool activeSelf;
    public float disableTime=1.5f;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<Collider2D>();
    }
    
    private void OnEnable()
    {
        _collider2D.enabled = true;
        StartCoroutine(DisableBullet());
    }
    
    IEnumerator DisableBullet()
    {
        yield return new WaitForSeconds(disableTime);
    }
    
    public void Fire(Vector3 dir, float speed)
    {
        //addforce 방식
        _rigidbody2D.AddForce(dir * speed, ForceMode2D.Impulse);
    }
    
    private void OnDisable()
    {
        _collider2D.enabled = false;
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