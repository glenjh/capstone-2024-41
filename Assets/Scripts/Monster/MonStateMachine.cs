using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonStateMachine {
    public IMonState currentState;
    public List<IMonState> states;
    Monster _mon;
    
    public void Init(Monster mon)
    {
        _mon = mon;
    }

    public void SetState(MonStateType stateType)
    {
        if(_mon.gameObject.activeSelf == false) return;
        if(_mon.isActing) return;
        if (currentState != null)
            currentState.Exit(_mon);
        
        
        _mon.stateType = stateType;
        currentState = MonManager.GetState(stateType);
        currentState.Enter(_mon);
    }
    

    public void Action(Monster mon)
    {
        if (currentState != null)
        {
            currentState.Action(mon);
        }
    }
}