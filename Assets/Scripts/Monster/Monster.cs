using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum MoveType
{
    Walk,
    Fly,
}

public abstract class Monster : MonoBehaviour, IDamageAble {
    public MonType monsterType;
    public int health;
    public int maxHealth;
    public int attackDamage;
    public float attackRange;
    public float moveSpeed;
    public MonStateType stateType;
    public MoveType moveType = MoveType.Walk;
    public float attackDelay = 1;
    public bool attackable = true;

    public Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public MonStateMachine StateMachine;
    
    public Transform player;

    [SerializeField] private Collider2D attackCollider;
    //죽었을때 사용할 콜백
    public Action OnDieCallBack;

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        StateMachine = new MonStateMachine();
        StateMachine.Init(this);
    }

    public virtual void Start()
    {
        Init();
    }

    public virtual void FixedUpdate()
    {
        StateMachine.Action(this);
        if(StateMachine.currentState.StateType == MonStateType.Die)
            return;
    }

    public virtual void Init()
    {
        attackCollider.enabled = false;
        health = maxHealth;
        StateMachine.SetState(MonStateType.Idle);
    }

    public virtual void TakeDamage(int damage)
    {
        // 몬스터가 피해를 받을 때 동작 구현
        health-=damage;
        if(health <= 0)
        {
            StateMachine.SetState(MonStateType.Die);
        }
    }
    
    public virtual void StartWait()
    {
        StartCoroutine("Wait");
    }
    
    public void StopWait()
    {
        StopCoroutine("Wait");
    }

    public void Chase(Transform target)
    {
        if (StateMachine.currentState.StateType == MonStateType.Move ||
            StateMachine.currentState.StateType == MonStateType.Idle)
        {
            StateMachine.SetState(MonStateType.Chase);
            ((MonStateChase)StateMachine.currentState).target = target;
        }
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(2f);
        // 2초 후에 실행할 코드
        if (UnityEngine.Random.Range(0, 2) == 0)
            StateMachine.SetState(MonStateType.Idle);
        else
            StateMachine.SetState(MonStateType.Move);
    }

    public virtual void Die()
    {
        OnDieCallBack?.Invoke();
        transform.parent.GetComponent<MonSpawner>()?.ReSpawn();
        // 몬스터가 죽을 때 동작 구현
        MonsterPool.ReturnObject(this);
    }
    
    public virtual void ToIdle()
    {
        StateMachine.SetState(MonStateType.Idle);
    }
    
    public virtual void OnAttack()
    {
        attackCollider.enabled = true;
    }
    
    public virtual void OffAttack()
    {
        attackCollider.enabled = false;
    }

    public void TakeHit(int damage, Transform target)
    {
        if(StateMachine.currentState.StateType == MonStateType.Die)
            return;
        health -= damage;
        StartCoroutine(OnDamage());
        if(health <= 0)
        {
            StateMachine.SetState(MonStateType.Die);
        }
    }
    IEnumerator OnDamage()
    {
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        
        yield return new WaitForSeconds(0.1f);
        
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public virtual void DecideAttack()
    {
        if (!attackable)
            return;
        var rayRange = Physics2D.Raycast(rb.position + new Vector2(0,-spriteRenderer.size.y/2), (moveSpeed > 0 ? Vector3.right : Vector3.left),
            attackRange, LayerMask.GetMask("Player"));
        if (rayRange.collider && StateMachine.currentState.StateType != MonStateType.Attack)
        {
            StateMachine.SetState(MonStateType.Attack);
            attackable = false;
        }
    }
    
    //어택 딜레이
    public void OnAttackDelay()
    {
        StartCoroutine(AttackDelayCoroutine());
    }
    
    IEnumerator AttackDelayCoroutine()
    {
        yield return new WaitForSeconds(attackDelay);
        Debug.Log("attackable");
        attackable = true;
    }
}