using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wasp : Monster
{
    public override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    public override void Start()
    {
        //Init();
    }
    
    public override void Init(string name, bool isActor = false)
    {
        monsterName = name;
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

    public override bool DecideChase()
    {
        //원 범위내에 플레이어가 있는지 확인
        var rayRange = Physics2D.CircleCast(transform.position, monsterSO.chaseRange, 
            Vector2.zero, 0, LayerMask.GetMask("Player"));
        if (rayRange.collider)
        {
            if(StateMachine.currentState.StateType != MonStateType.Chase)
                StateMachine.SetState(MonStateType.Chase);
            ((MonStateChase)StateMachine.currentState).target = rayRange.collider.transform;
            return true;
        }

        return false;
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }
}