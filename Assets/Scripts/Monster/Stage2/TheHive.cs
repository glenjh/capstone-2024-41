using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheHive : Monster
{
    [SerializeField] private int curWaspCount = 0;
    [SerializeField] private int maxWaspCount = 3;

    public override void Init(string name, bool isActor = false)
    {
        monsterName = name;
        //attackCollider.enabled = false;
        health = monsterSO.maxHealth;
        StateMachine.SetState(MonStateType.Idle);
        attackable = true;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public override void StartWait()
    { }

    public override bool DecideAttack()
    {
        if (!attackable)
            return false;
        
        if (curWaspCount < maxWaspCount)
        {
            SpwanWasp();
            attackable = false;
            OnAttackDelay();
        }

        return false;
    }
    
    public void SpwanWasp()
    {
        Monster monster = PoolManager.Instance.GetFromPool<Monster>("Wasp");
        monster.transform.position = transform.position;
        monster.player = player;
        monster.Init("Wasp");
        monster.OnDieCallBack = ReSpawnWasp;
        
        curWaspCount++;
    }
    
    public void ReSpawnWasp()
    {
        curWaspCount--;
    }
}