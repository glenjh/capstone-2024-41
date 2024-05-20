using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine
{
    Player player;
    Dictionary<PlayerStates, PlayerState> states = new Dictionary<PlayerStates, PlayerState>();
    PlayerState currState;

    public PlayerStateMachine(PlayerStates ps, Player player)
    {
        this.player = player;
        AddState(ps);
        ChangeState(ps);
    }

    public void AddState(PlayerStates ps)
    {
        if (!states.ContainsKey(ps))
        {
            states.Add(ps, StateManager.GetState(ps));
        }
    }
    
    public void ChangeState(PlayerStates ps)
    {
        currState?.Exit(player);
        if (!states.ContainsKey(ps))
        {
            return;
        }
        currState = states[ps];
        player.PS = ps;
        currState.Enter(player);
    }
    
    public void Action()
    {
        currState.Update(player);
    }

    public PlayerState GetCurrState()
    {
        return currState;
    }

    public void DeleteState(PlayerStates ps)
    {
        states.Remove(ps);
    }
}

public static class StateManager
{
    public static PlayerState GetState(PlayerStates ps)
    {
        switch (ps)
        {
            case PlayerStates.IDLE:
                return new Idle(ps);
            
            case PlayerStates.MOVE:
                return new Move(ps);
            
            case PlayerStates.JUMP:
                return new Jump(ps);
            
            case PlayerStates.ATTACK:
                return new Attack(ps);
            
            case PlayerStates.SLASHATTACK:
                return new SlashAttack(ps);
            
            case PlayerStates.JUMPATTACK:
                return new JumpAttack(ps);

            case PlayerStates.STAMPING:
                return new Stamping(ps);
            
            case PlayerStates.DASH:
                return new Dash(ps);
            
            case PlayerStates.PARRYING: // 추후에 픽업
                return new Parry(ps);
            
            case PlayerStates.WALLSLIDING: // 추후에 픽업
                return new Wallsliding(ps);
            
            case PlayerStates.WALLJUMPING:  // 추후에 픽업
                return new Walljumping(ps);

            case PlayerStates.DEAD:
                return new Dead(ps);
            
            default:
                return null;
        }
    }
}
