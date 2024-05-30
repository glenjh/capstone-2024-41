using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossStateType
{
    Sleep,
    Wake,
    Idle,
    Move,
    Attack,
    Dead,
    Buff
}

public interface IBossState {
    public BossStateType StateType { get; }
    public void Enter(Boss boss)
    {
    }
    
    public void Action(Boss boss)
    {
    }
    
    public void Exit(Boss boss)
    {
    }
}

public class BossStateIdle : IBossState
{
    BossStateType _stateType;
    public BossStateType StateType => _stateType;
    //생성자
    public BossStateIdle()
    {
        _stateType = BossStateType.Idle;
    }
    
    public void Enter(Boss boss)
    {
        boss.animator.SetBool("isMove", false);
        boss.OnMove();
    }
    
    public void Action(Boss boss)
    {
        if(boss.player == null) return;
    }
    
    public void Exit(Boss boss)
    {
        boss.OffMove();
    }
}

public class BossStateSleep : IBossState
{
    BossStateType _stateType;
    public BossStateType StateType => _stateType;
    //생성자
    public BossStateSleep()
    {
        _stateType = BossStateType.Sleep;
    }
    
    public void Enter(Boss boss)
    {
        boss.isSuperArmor = true;
        boss.animator.SetBool("isWake", false);
        boss.rb.velocity = Vector3.zero;
    }
    
    public void Action(Boss boss)
    {
        
    }
    
    public void Exit(Boss boss)
    {
        //boss.animator.SetBool("isWake", true);
        boss.isSuperArmor = false;
    }
}

public class BossStateWake : IBossState
{
    BossStateType _stateType;
    public BossStateType StateType => _stateType;
    //생성자
    public BossStateWake()
    {
        _stateType = BossStateType.Wake;
    }
    
    public void Enter(Boss boss)
    {
        boss.isSuperArmor = true;
        boss.animator.SetTrigger("DoWake");
    }
    
    public void Action(Boss boss)
    {
    }
    
    public void Exit(Boss boss)
    {
        boss.attackCollider.enabled = true;
        boss.isSuperArmor = false;
        boss.animator.SetBool("isWake", true);
    }
}

public class BossStateMove :IBossState
{
    BossStateType _stateType;
    bool _dirRight = true;
    public BossStateType StateType => _stateType;
    private bool haveTurn = false;
    //생성자
    public BossStateMove()
    {
        _stateType = BossStateType.Move;
    }
    
    public void Enter(Boss boss)
    {
        boss.animator.SetBool("isMove", true);
        _dirRight = boss.transform.localScale.x > 0;
        foreach (var parm in boss.animator.parameters)
        {
            if (parm.name == "DoTurn")
            {
                haveTurn = true;
                break;
            }
        }
    }
    
    public void Action(Boss boss)
    {
        // 플레이어와 보스의 위치 비교하여 방향 설정
        bool isPlayerOnRight = boss.player.position.x > boss.transform.position.x;

        // 플레이어의 방향과 보스의 현재 방향이 다를 때만 방향 전환 처리
        if (_dirRight != isPlayerOnRight)
        {
            _dirRight = isPlayerOnRight;
            boss.transform.localScale = new Vector3(-1*boss.transform.localScale.x, boss.transform.localScale.y, boss.transform.localScale.z);
            if(haveTurn == true)
                boss.animator.SetTrigger("DoTurn");
        }

        if (boss.minAttackRange == 0 || Mathf.Abs(boss.player.position.x - boss.transform.position.x) < boss.minAttackRange)
        {
            boss.rb.velocity = new Vector2(0, boss.rb.velocity.y);
            boss.StateMachine.SetState(BossStateType.Attack);
            return;
        }

        boss.rb.velocity = new Vector2(_dirRight ? boss.moveSpeed : -boss.moveSpeed, boss.rb.velocity.y);
    }
    
    public void Exit(Boss boss)
    {
        boss.animator.SetBool("isMove", false);  
    }
}

public class BossStateAttack : IBossState
{
    BossStateType _stateType;
    public BossStateType StateType => _stateType;
    //생성자
    public BossStateAttack() => _stateType = BossStateType.Attack;
    
    public void Enter(Boss boss)
    {
        boss.rb.velocity = Vector3.zero;
        boss.animator.SetTrigger("DoAttack");
        boss.OnAttack();
    }
    
    public void Action(Boss boss)
    {
        // 상태가 지속될 때 동작 구현
    }
    
    public void Exit(Boss boss)
    {
        // 상태가 종료될 때 동작 구현
        boss.attackCollider.enabled = false;
    }
}

public class BossStateDead : IBossState
{
    BossStateType _stateType;

    public BossStateType StateType => _stateType;

    //생성자
    public BossStateDead() => _stateType = BossStateType.Dead;

    public void Enter(Boss boss)
    {
        // boss.animator.SetTrigger("DoDie");
        boss.attackCollider.enabled = false;
        boss.dieTimeLine.Play();
        boss.Dead();
    }

    public void Action(Boss boss)
    {
        // 상태가 지속될 때 동작 구현
    }

    public void Exit(Boss boss)
    {
        // 상태가 종료될 때 동작 구현
    }
}

public class BossStateBuff : IBossState
{
    BossStateType _stateType;

    public BossStateType StateType => _stateType;

    //생성자
    public BossStateBuff() => _stateType = BossStateType.Buff;

    public void Enter(Boss boss)
    {
        boss.attackCollider.enabled = false;
        boss.animator.SetTrigger("DoBuff");
        boss.OnBuff();
    }

    public void Action(Boss boss)
    {
        // 상태가 지속될 때 동작 구현
    }

    public void Exit(Boss boss)
    {
        boss.attackCollider.enabled = true;
    }
}