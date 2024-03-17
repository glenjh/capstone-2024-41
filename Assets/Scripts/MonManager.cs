using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonStateType
{
    Idle,
    Move,
    Chase,
    Attack,
    Die
}
public static class MonManager {
    public static IMonState GetState(MonStateType t)
    {
        switch (t)
        {
            case MonStateType.Idle:
                return new MonStateIdle();
            case MonStateType.Move:
                return new MonStateMove();
            case MonStateType.Chase:
                return new MonStateChase();
            case MonStateType.Die:
                return new MonStateDie();
            case MonStateType.Attack:
                return new MonStateAttack();
            
        }
        return null;
    }
}