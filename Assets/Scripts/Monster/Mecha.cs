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

    public void ReadyAttack()
    {
        animator.SetBool("isAttack", false);
    }
    
    public override void ToIdle()
    {
        StateMachine.SetState(MonStateType.Idle);
    }
}