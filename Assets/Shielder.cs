using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shielder : Monster {
    //근접공격 범위
    public float meleeRange;
    private static readonly int ChoiseAttack = Animator.StringToHash("choiceAttack");

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

    // Update is called once per frame
    public override void FixedUpdate() {
        base.FixedUpdate();
        if (StateMachine.currentState.StateType == MonStateType.Die)
            return;
        DecideAttack();
    }

    private void DecideAttack()
    {
        //공격 범위 내로 플레이어가 들어오면 공격
        RaycastHit2D rayRange = Physics2D.Raycast(rb.position + Vector2.down, (moveSpeed >0 ? Vector3.right: Vector3.left), attackRange, LayerMask.GetMask("Player"));
        if (rayRange.collider != null && StateMachine.currentState.StateType != MonStateType.Attack)
        {
            //meleeRange 내로 플레이어가 들어오면 근접공격
            RaycastHit2D rayMelee = Physics2D.Raycast(rb.position + Vector2.down, (moveSpeed >0 ? Vector3.right: Vector3.left),
                meleeRange, LayerMask.GetMask("Player"));
            if (rayMelee.collider != null)
            {
                animator.SetInteger(ChoiseAttack, 0);
            }
            else
            {
                animator.SetInteger(ChoiseAttack, 1);
            }

            StateMachine.SetState(MonStateType.Attack);
        }
    }
}