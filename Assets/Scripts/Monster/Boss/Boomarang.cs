using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomarang : MonoBehaviour {
    Rigidbody2D _rigidbody2D;
    [SerializeField] Transform _player;
    AncientBoss _boss;
    Vector2 _originPos;
    
    void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }
    
    private void OnEnable()
    {
        if(_boss == null)
            _boss = GetComponentInParent<AncientBoss>();
        if(_player == null)
            _player = _boss.player;
        StartCoroutine(BoomarangOn());
    }

    IEnumerator BoomarangOn()
    {
        _originPos = transform.position;
        float currentTime = 0;
        float maxAttakTime = 3f;
        float maxBackTime = 1.5f;
        while (true)
        {
            currentTime += Time.deltaTime;
            //x축만 이동
            if(transform.position.x - _player.position.x < 0.1f && transform.position.x - _player.position.x > -0.1f)
                _rigidbody2D.velocity = Vector2.zero;
            else if(transform.position.x > _player.position.x)
                _rigidbody2D.velocity = Vector2.left * 5f;
            else
                _rigidbody2D.velocity = Vector2.right * 5f;
            if (currentTime >= maxAttakTime)
            {
                break;
            }
            
            yield return null;
        }
        _rigidbody2D.velocity = Vector2.zero;
        currentTime = 0;
        while (true)
        {
            currentTime += Time.deltaTime;
            if (transform.position.x - _originPos.x < 0.1f && transform.position.x - _originPos.x > -0.1f)
                break;
            else if(transform.position.x > _originPos.x)
                _rigidbody2D.velocity = Vector2.left * 10f;
            else
                _rigidbody2D.velocity = Vector2.right * 10f;
            if (currentTime >= maxBackTime)
                break;

            yield return null;
        }
        _rigidbody2D.velocity = Vector2.zero;
        transform.position = _originPos;
        _boss.OffSpinAttack();
        _boss.OnIdle();
    }
}