using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public enum BossType
{
    AncientBoss,
    TheWidow,
    ShadowOfStorms,
}

public abstract class Boss : MonoBehaviour, IDamageAble {
    public BossType monsterType;
    public int health;
    public int maxHealth;
    public int attackDamage;
    public float moveSpeed;
    public BossStateType stateType;
    public BossHealthBar healthBar;
    public float minAttackRange = 1f;
    [SerializeField] 
    public PlayableDirector dieTimeLine;
    
    public bool isSuperArmor = true;

    public Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public BossStateMachine StateMachine;
    public BoxCollider2D attackCollider;
    private BoxCollider2D bossCollider;
    
    public Transform player;
    public bool isDonMove = true;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        StateMachine = new BossStateMachine();
        bossCollider = GetComponent<BoxCollider2D>();
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
    }
    
    public virtual void TakeHit(int damage, Transform transform)
    {
        if(isSuperArmor||StateMachine.currentState.StateType == BossStateType.Dead)
            return;
        // 몬스터가 피해를 받을 때 동작 구현
        health-=damage;
        // healthBar.SetHealth(health);
        var effect = PoolManager.Instance.GetFromPool<EffectSystem>("MonsterHitEffect");
        effect.transform.position = this.transform.position;
        if(health <= 0)
        {
            StateMachine.SetState(BossStateType.Dead);
            healthBar.HideSignal();
        }
    }

    public virtual void OnBuff() {}

    public void OnIdle() => StateMachine.SetState(BossStateType.Idle);

    IEnumerator OnTimer()
    {
        yield return new WaitForSeconds(2f);
        attackCollider.enabled = true;
        
        if(!isDonMove)
            StateMachine.SetState(BossStateType.Move);
    }
    
    public void OnMove() => StartCoroutine(OnTimer());

    public void OffMove() => StopCoroutine(OnTimer());

    public virtual void OnAttack(){}
    
    public void WakeUp()
    {
        StateMachine.SetState(BossStateType.Wake);
    }

    public virtual void Dead()
    {
        bossCollider.enabled = false;
        //AchiveManager.BossDefeated(monsterType.ToString());
    }
    
    public virtual void OffAttack()
    { }
    
    public void AbleMove()
    {
        isDonMove = false;
        StateMachine.SetState(BossStateType.Move);
    }
}