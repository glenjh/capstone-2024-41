using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class Monster : MonoBehaviour {
    public string monsterName;
    public int health;
    public int attackDamage;
    public float attackRange;
    public float moveSpeed;

    public Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public MonStateMachine stateMachine;

    public virtual void Awake()
    {
        Init();
    }

    public virtual void Start()
    {
        stateMachine.SetState(MonStateType.Move);
    }

    public virtual void FixedUpdate()
    {
        stateMachine.Action(this);
    }

    public virtual void Init()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        stateMachine = new MonStateMachine();
        
        stateMachine.Init(this);
    }

    public virtual void Attack()
    {
        // 몬스터의 공격 동작 구현
    }

    public virtual void TakeDamage(int damage)
    {
        // 몬스터가 피해를 받을 때 동작 구현
        health-=damage;
    }
    
    public void StartWait()
    {
        StartCoroutine(Wait());
    }
    
    public void StopWait()
    {
        StopCoroutine(Wait());
    }

    public void Chase(Transform target)
    {
        stateMachine.SetState(MonStateType.Chase);
        ((MonStateChase)stateMachine.currentState).target = target;
    }
    
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(2f);
        // 2초 후에 실행할 코드
        if (UnityEngine.Random.Range(0, 2) == 0)
            stateMachine.SetState(MonStateType.Idle);
        else
            stateMachine.SetState(MonStateType.Move);
    }

    public virtual void Die()
    {
        // 몬스터가 죽을 때 동작 구현
        Destroy(gameObject);
    }

}