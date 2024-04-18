using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheHive : Monster
{
    [SerializeField] private int curWaspCount = 0;
    [SerializeField] private int maxWaspCount = 3;
    
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
    
    public override void StartWait()
    { }

    public override void DecideAttack()
    {
        if (curWaspCount < maxWaspCount)
        {
            SpwanWasp();
        }
    }
    
    public void SpwanWasp()
    {
        Monster monster = MonsterPool.GetObject(MonType.Wasp);
        monster.transform.position = transform.position;
        monster.player = player;
        monster.OnDieCallBack = ReSpawnWasp;
        
        curWaspCount++;
    }
    
    public void ReSpawnWasp()
    {
        curWaspCount--;
    }
}