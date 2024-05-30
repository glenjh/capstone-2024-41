using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkMec : Monster {
    [SerializeField] Transform originBulletPos;
    [SerializeField] Bullet bullet;
    [SerializeField] float bulletSpeed;
    [SerializeField] private float ySize;
    
    public override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    public override void Start() {
        base.Start();
    }
    
    public override void Init(string name, bool isActor = false)
    {
        monsterName = name;
        //attackCollider.enabled = false;
        health = monsterSO.maxHealth;
        StateMachine.SetState(MonStateType.Idle);
        attackable = true;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    // Update is called once per frame
    public override void FixedUpdate() {
        base.FixedUpdate();
        if (StateMachine.currentState.StateType == MonStateType.Die)
            return;
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }

    public override bool DecideAttack()
    {
        if (bullet.activeSelf)
            return false;
        if (!attackable)
            return false;
        Debug.DrawRay(rb.position + new Vector2(0,-ySize), (moveSpeed > 0 ? Vector3.right : Vector3.left) * monsterSO.attackRange, Color.red);
        var rayRange = Physics2D.Raycast(rb.position + new Vector2(0,-ySize), (moveSpeed > 0 ? Vector3.right : Vector3.left),
            monsterSO.attackRange, LayerMask.GetMask("Player"));
        if (rayRange.collider && StateMachine.currentState.StateType != MonStateType.Attack)
        {
            StateMachine.SetState(MonStateType.Attack);
            attackable = false;
            return true;
        }

        return false;
    }

    public override bool DecideChase()
    {
        var rayRange = Physics2D.Raycast(rb.position + new Vector2(0,-ySize), (moveSpeed > 0 ? Vector3.right : Vector3.left),
            monsterSO.chaseRange, LayerMask.GetMask("Player"));
        if (rayRange.collider)
        {
            if(StateMachine.currentState.StateType != MonStateType.Chase)
                StateMachine.SetState(MonStateType.Chase);
            ((MonStateChase)StateMachine.currentState).target = rayRange.collider.transform;
            return true;
        }

        return false;
    }

    public override void OnAttack()
    {
        // 총알을 생성하고 발사할 방향 설정
        Bullet _bullet = Instantiate(bullet).GetComponent<Bullet>();
        
        _bullet.transform.position = originBulletPos.position;
        //좌우 판단
        _bullet.Fire(moveSpeed > 0 ? Vector2.right : Vector2.left, bulletSpeed);
    }
}