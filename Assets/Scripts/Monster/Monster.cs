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

public abstract class Monster : MonoBehaviour, IDamageAble{
    public MonsterSO monsterSO;

    public string monsterName;
    public int health;
    public MonStateType stateType;
    public bool attackable = true;
    public float moveSpeed;
    
    public Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public MonStateMachine StateMachine;
    
    public Transform player;

    [SerializeField] private Collider2D ChaseCollider;
    [SerializeField] private Collider2D attackCollider;
    //죽었을때 사용할 콜백
    public Action OnDieCallBack;
    
    [SerializeField]private FragmentsSystem fS;
    private Transform impactPos;
    
    [SerializeField] private BezierMissileShooter missileShooter;
    
    [SerializeField] private bool isActor = false;
    public bool isActing = false;
    public bool isDamaged = false;
    public bool isLeft = false;

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        StateMachine = new MonStateMachine();
        StateMachine.Init(this);

        Init("");
        isActing = isActor;
    }

    public virtual void Start()
    {
    }

    public virtual void FixedUpdate()
    {
        if(isActing)
            return;
        StateMachine.Action(this);
    }

    public virtual void Init(string name, bool isActor = false)
    {
        moveSpeed = monsterSO.maxMoveSpeed;
        monsterName = name;
        attackCollider.enabled = false;
        health = monsterSO.maxHealth;
        StateMachine.SetState(MonStateType.Idle);
        attackable = true;
        spriteRenderer.color = new Color(1, 1, 1, 1);
        missileShooter.m_shotCount = 1;
        isDamaged = false;

        if (this.isActor)
            return;
        else
        {
            this.isActor = isActor;
            transform.localScale = new Vector3(1, 1, 1);
        }
            
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
        TakeHit(1000, transform);
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
        if(health<=0)
            return;
        
        health -= damage;
        EffectSystem effect;
        if(health <= 0)
        {
            if(missileShooter!=null)
                missileShooter.Fire(3);
            fS.InstantiateFragment(target,transform,damage);
            StateMachine.SetState(MonStateType.Die);
            //effect = CameraManager.instance._poolManager.GetFromPool<EffectSystem>("MonsterDeadEffect");
            //effect.transform.position = transform.position;
            
            if(!isActor)
                PoolManager.Instance.TakeToPool<Monster>(monsterName,this);
            else
                gameObject.SetActive(false);
            return;
        }
        effect = PoolManager.Instance.GetFromPool<EffectSystem>("MonsterHitEffect");
        if(missileShooter!=null)
            missileShooter.Fire();
        if(effect!=null)
            effect.transform.position = transform.position;
        
        //좌우 판단 후 넉백
        if(target.position.x < transform.position.x)
        {
            rb.velocity =new Vector2(1.5f, 1.5f);
            if(moveSpeed > 0)
                Turn();
        }
        else
        {
            rb.velocity =new Vector2(-1.5f, 1.5f);
            if(moveSpeed < 0)
                Turn();
        }
        
        StartCoroutine(OnDamage());
    }
    IEnumerator OnDamage()
    {
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        isDamaged = true;
        yield return new WaitForSeconds(0.1f);
        isDamaged = false;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public virtual bool DecideAttack()
    {
        if (!attackable||isActing)
            return false;
        Debug.DrawRay(rb.position + new Vector2(0,-spriteRenderer.size.y), (moveSpeed > 0 ? Vector3.right : Vector3.left) * monsterSO.attackRange, Color.red);
        var rayRange = Physics2D.Raycast(rb.position + new Vector2(0,-spriteRenderer.size.y), (moveSpeed > 0 ? Vector3.right : Vector3.left),
            monsterSO.attackRange, LayerMask.GetMask("Player"));
        if (rayRange.collider && StateMachine.currentState.StateType != MonStateType.Attack)
        {
            StateMachine.SetState(MonStateType.Attack);
            attackable = false;
            return true;
        }

        return false;
    }

    public virtual bool DecideChase()
    {
        var rayRange = Physics2D.Raycast(rb.position + new Vector2(0,-spriteRenderer.size.y), (moveSpeed > 0 ? Vector3.right : Vector3.left),
            monsterSO.chaseRange, LayerMask.GetMask("Player"));
        Debug.DrawRay(rb.position + new Vector2(0,-spriteRenderer.size.y), (moveSpeed > 0 ? Vector3.right : Vector3.left) * monsterSO.chaseRange, Color.green);
        if (rayRange.collider)
        {
            if(StateMachine.currentState.StateType != MonStateType.Chase)
                StateMachine.SetState(MonStateType.Chase);
            ((MonStateChase)StateMachine.currentState).target = rayRange.collider.transform;
            return true;
        }

        return false;
    }
    
    //어택 딜레이
    public void OnAttackDelay()
    {
        StartCoroutine(AttackDelayCoroutine());
    }
    
    IEnumerator AttackDelayCoroutine()
    {
        yield return new WaitForSeconds(monsterSO.attackDelay);
        attackable = true;
    }

    public void Turn()
    {
        moveSpeed *= -1;
        transform.localScale = new Vector3(-1 * transform.localScale.x,
            transform.localScale.y, transform.localScale.z);
    }

    public void EnableChase()
    {
        isActing = false;
        if (isLeft)
        {
            moveSpeed *= -1;
        }
        Chase(player);
    }
}