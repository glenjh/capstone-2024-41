using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallTank : Monster {
    public override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    public override void Start()
    {
        Init();
    }
    
    public override void Init()
    {
        health = maxHealth;
        StateMachine.SetState(MonStateType.Idle);
    }

    public override void StartWait()
    { }

    // Update is called once per frame
    public override void FixedUpdate() {
        base.FixedUpdate();
        if (StateMachine.currentState.StateType == MonStateType.Die)
            return;
        //공격 범위 내로 플레이어가 들어오면 공격
        RaycastHit2D rayRange = Physics2D.Raycast(rb.position - new Vector2(attackRange/2,0), Vector2.right, attackRange, LayerMask.GetMask("Player"));
        if (rayRange.collider != null && StateMachine.currentState.StateType != MonStateType.Attack)
        {
            StateMachine.SetState(MonStateType.Attack);
        }
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }
}