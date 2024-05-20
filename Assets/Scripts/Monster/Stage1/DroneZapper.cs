using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneZapper : Monster {
    
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
        if (StateMachine.currentState.StateType == MonStateType.Die)
            return;
        //공격 범위 내로 플레이어가 들어오면 공격
        Debug.DrawRay(rb.position, (moveSpeed >0 ? Vector3.right: Vector3.left), new Color(0, 1, 0));
        
        DecideAttack();
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }

    public override bool DecideAttack()
    {
        if (!attackable)
        {
            return false;
        }
        RaycastHit2D rayRange = Physics2D.Raycast(rb.position, (moveSpeed > 0 ? Vector3.right : Vector3.left),
            attackRange, LayerMask.GetMask("Player"));
        if (rayRange.collider != null && StateMachine.currentState.StateType != MonStateType.Attack)
        {
            StateMachine.SetState(MonStateType.Attack);
            attackable = false;
            return true;
        }
        return false;
    }

    public void ReadyAttack()
    {
        animator.SetBool("isAttack", false);
    }
}