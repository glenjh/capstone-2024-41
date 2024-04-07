using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossType
{
    AncientBoss
}

public abstract class Boss : MonoBehaviour, IDamageAble {
    public BossType monsterType;
    public int health;
    public int maxHealth;
    public int attackDamage;
    public float moveSpeed;
    public BossStateType stateType;
    public float awakeRange;
    
    public bool isSuperArmor;

    public Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public BossStateMachine StateMachine;
    public BoxCollider2D attackCollider;
    
    public Transform player;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        StateMachine = new BossStateMachine();
    }

    // Start is called before the first frame update
    protected virtual void Start() {
        Init();
    }

    // Update is called once per frame
    protected virtual void FixedUpdate() {
        StateMachine.Action(this);
    }

    public virtual void Init()
    {
        StateMachine.Init(this);
        attackCollider.enabled = false;
        StateMachine.addState(BossStateType.Sleep, new BossStateSleep());
        StateMachine.addState(BossStateType.Wake, new BossStateWake());
        StateMachine.addState(BossStateType.Idle, new BossStateIdle());
        StateMachine.addState(BossStateType.Move, new BossStateMove());
        StateMachine.addState(BossStateType.Attack, new BossStateAttack());
        StateMachine.addState(BossStateType.Dead, new BossStateDead());
        StateMachine.addState(BossStateType.Buff, new BossStateBuff());

        StateMachine.SetState(BossStateType.Sleep);
    }
    
    public virtual void TakeHit(int damage, Transform transform)
    {
        if(isSuperArmor)
            return;
        // 몬스터가 피해를 받을 때 동작 구현
        health-=damage;
        if(health <= 0)
        {
            StateMachine.SetState(BossStateType.Dead);
        }
    }

    public virtual void OnBuff() {}

    public void OnIdle() => StateMachine.SetState(BossStateType.Idle);

    IEnumerator OnTimer()
    {
        yield return new WaitForSeconds(2f);
        attackCollider.enabled = true;
        StateMachine.SetState(BossStateType.Move);
    }
    
    public void OnMove() => StartCoroutine(OnTimer());

    public void OffMove() => StopCoroutine(OnTimer());

    public virtual void OnAttack(){}
}