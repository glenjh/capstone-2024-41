using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mecha : Monster {
    
    public override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    public override void Start() {
        base.Start();
    }

    // Update is called once per frame
    public override void FixedUpdate() {
        base.FixedUpdate();
        //공격 범위 내로 플레이어가 들어오면 공격
        Debug.DrawRay(rb.position, (moveSpeed >0 ? Vector3.right: Vector3.left), new Color(0, 1, 0));
        RaycastHit2D rayRange = Physics2D.Raycast(rb.position, (moveSpeed >0 ? Vector3.right: Vector3.left), attackRange, LayerMask.GetMask("Player"));
        if (rayRange.collider != null && stateMachine.currentState.StateType != MonStateType.Attack)
        {
            stateMachine.SetState(MonStateType.Attack);
        }
        
        if(Input.GetKeyDown(KeyCode.Space))
        {
            stateMachine.SetState(MonStateType.Attack);
        }
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if(health <= 0)
        {
            stateMachine.SetState(MonStateType.Die);
        }
    }

    public void ReadyAttack()
    {
        animator.SetBool("isAttack", false);
    }
    
    public void ToIdle()
    {
        stateMachine.SetState(MonStateType.Idle);
    }
}