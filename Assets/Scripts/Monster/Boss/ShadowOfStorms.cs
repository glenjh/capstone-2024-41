using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowOfStorms : Boss
{
    private List<Action> attackActions = new List<Action>();
    private Monster[] _monsters;
    protected override void Start()
    {
        base.Start();
        
        StateMachine.SetState(BossStateType.Sleep);
        
        attackActions.Add(Heal);
        attackActions.Add(SimpleMeleeAttack);
        attackActions.Add(ComboMeleeAttack);
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
            StateMachine.SetState(BossStateType.Dead);
    }
    
    public override void OnAttack()
    {
        //리스트내에 있는 함수를 임의로 실행
        attackActions[UnityEngine.Random.Range(0, attackActions.Count)]();
    }

    #region Attack

    private void Heal()
    {
        animator.SetInteger("AttackType", 0);
        if(_monsters != null&&_monsters.Length != 0)
        {
            foreach (var monster in _monsters)
            {
                monster.health += 5;
            }
        }
        health += 5;
        if (health > maxHealth)
            health = maxHealth;
    }
    
    private void SimpleMeleeAttack()
    {
        animator.SetInteger("AttackType", 2);
    }
    
    private void ComboMeleeAttack()
    {
        animator.SetInteger("AttackType", 1);
    }

    #endregion
}
