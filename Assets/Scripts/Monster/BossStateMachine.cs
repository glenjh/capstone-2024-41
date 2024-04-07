using System.Collections.Generic;

public class BossStateMachine
{
    public IBossState currentState;
    public Dictionary<BossStateType, IBossState> states = new Dictionary<BossStateType, IBossState>();
    Boss _boss;

    public void Init(Boss boss)
    {
        _boss = boss;
    }

    public void addState(BossStateType bossStateType, IBossState state)
    {
        states.Add(bossStateType, state);
    }

    public void SetState(BossStateType stateType)
    {
        if (_boss.gameObject.activeSelf == false) return;
        if (currentState != null)
        {
            if(currentState.StateType == stateType) return; 
            currentState.Exit(_boss);
        }

        _boss.stateType = stateType;
        currentState = states[stateType];
        currentState.Enter(_boss);
    }


    public void Action(Boss boss)
    {
        if (currentState != null)
        {
            currentState.Action(boss);
        }
    }
}
