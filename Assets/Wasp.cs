using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wasp : Monster {
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
    {
        
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
}