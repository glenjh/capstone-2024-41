using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMonState {
    public MonStateType StateType { get; }
    public void Enter(Monster monster)
    {
    }
    
    public void Action(Monster monster)
    {
    }
    
    public void Exit(Monster monster)
    {
    }
}

public class MonStateIdle : IMonState
{
    MonStateType _stateType;
    
    public MonStateType StateType => _stateType;
    //생성자
    public MonStateIdle()
    {
        _stateType = MonStateType.Idle;
    }
    
    public void Enter(Monster monster)
    {
        if (monster.isActiveAndEnabled)
        {
            monster.animator.SetBool("isMove", false);
            monster.StartWait();
        }
    }
    
    public void Action(Monster monster)
    {        
        if(monster.moveType != MoveType.Fly)
        {
            if(monster.DecideAttack())
                return;
        }
        monster.DecideChase();
    }
    
    public void Exit(Monster monster)
    {
        monster.StopWait();
    }
}

public class MonStateMove :IMonState
{
    MonStateType _stateType;
    public MonStateType StateType => _stateType;
    //생성자
    public MonStateMove()
    {
        _stateType = MonStateType.Move;
    }
    public int nextMove;
    public void Enter(Monster monster)
    {
        // 상태가 시작될 때 동작 구현
        monster.animator.SetBool("isMove", true);
        Decide(monster);
        monster.StartWait();
    }
    
    public void Action(Monster monster)
    {
        if (monster.moveType == MoveType.Fly)
        {
            //x, y축 랜덤 이동
            monster.rb.velocity = new Vector2(monster.moveSpeed*-1f, Random.Range(-1f, 1f));
            monster.DecideChase();
            return;
        }
        // 다음 지형이 바닥인지 확인
        Vector2 frontVec = new Vector2(monster.rb.position.x + (monster.moveSpeed >0 ? 0.5f:-0.5f), monster.rb.position.y);
        int layerMask = (1 << LayerMask.NameToLayer("Ground")) + (1 << LayerMask.NameToLayer("HoverGround"));
        
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, monster.spriteRenderer.sprite.bounds.size.y/2+0.1f, layerMask);
        if (rayHit.collider == null)
        {
            monster.Turn();
        }
        
        monster.rb.velocity = new Vector2(monster.moveSpeed, monster.rb.velocity.y);
        if(monster.DecideAttack())
        {
            return;
        }
        monster.DecideChase();
    }

    //행동 결정 로직
    public void Decide(Monster monster)
    {
        // 몬스터가 이동할 방향 결정
        nextMove = Random.Range(0, 2);
        if (nextMove == 0)
        {
            monster.Turn();
        }
    }
    
    public void Exit(Monster monster)
    {
        // 상태가 종료될 때 동작 구현
        monster.animator.SetBool("isMove", false);
        monster.StopWait();
    }
}

public class MonStateAttack : IMonState
{
    MonStateType _stateType;
    public MonStateType StateType => _stateType;
    //생성자
    public MonStateAttack()
    {
        _stateType = MonStateType.Attack;
    }
    
    public void Enter(Monster monster)
    {
        // 상태가 시작될 때 동작 구현
        monster.animator.SetBool("isAttack", true);
        monster.animator.SetTrigger("doAttack");
        monster.rb.velocity = new Vector2(0, monster.rb.velocity.y);
    }
    
    public void Action(Monster monster)
    {
        // 상태가 지속될 때 동작 구현
    }
    
    public void Exit(Monster monster)
    {
        monster.animator.SetBool("isAttack", false);
        monster.OnAttackDelay();
        // 상태가 종료될 때 동작 구현
    }
}

public class MonStateDie : IMonState
{
    MonStateType _stateType;
    public MonStateType StateType => _stateType;
    //생성자
    public MonStateDie()
    {
        _stateType = MonStateType.Die;
    }
    public void Enter(Monster monster)
    {
        //monster.animator.SetTrigger("doDie");
        monster.spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        EffectSystem effect = PoolManager.Instance.GetFromPool<EffectSystem>("MonsterDeadEffect");
        effect.transform.position = monster.transform.position;
        monster.OnDieCallBack?.Invoke();
    }
    
    public void Action(Monster monster)
    {
        // 상태가 지속될 때 동작 구현
    }
    
    public void Exit(Monster monster)
    {
        // 상태가 종료될 때 동작 구현
    }
}

public class MonStateChase : IMonState
{
    MonStateType _stateType;
    public Transform target;
    public MonStateType StateType => _stateType;
    //생성자
    public MonStateChase()
    {
        _stateType = MonStateType.Chase;
    }
    public void Enter(Monster monster)
    {
        // 상태가 시작될 때 동작 구현
        monster.animator.SetBool("isMove", true);
        if(monster.moveType == MoveType.Fly)
        {
            monster.moveSpeed*=2;
        }
    }
    
    public void Action(Monster monster)
    {
        if(target == null)
            return;
        float gap = target.position.x - monster.transform.position.x;
        
        if (monster.moveType == MoveType.Fly)
        {
            var dir = (target.position - monster.transform.position).normalized;
            if (Mathf.Abs(gap) > 1f)
            {
                if (gap > 0 && monster.moveSpeed > 0)
                {
                    monster.Turn();
                }
                else if (gap < 0 && monster.moveSpeed < 0)
                {
                    monster.Turn();
                }
            }
            monster.rb.velocity = dir * Mathf.Abs(monster.moveSpeed);
            
            if(!monster.DecideChase())
                monster.StateMachine.SetState(MonStateType.Idle);
            
            return;
        }

        if (Mathf.Abs(gap) > monster.attackRange)
        {
            if (gap > 0 && monster.moveSpeed < 0)
            {
                monster.Turn();
            }
            else if (gap < 0 && monster.moveSpeed > 0)
            {
                monster.Turn();
            }
            monster.rb.velocity = new Vector2(monster.moveSpeed, monster.rb.velocity.y);
        }
        else
        {
            //monster.StateMachine.SetState(MonStateType.Idle);
            monster.rb.velocity = new Vector2(0, monster.rb.velocity.y);
        }

        if(monster.DecideAttack())
        {
            return;
        }
        if (!monster.DecideChase())
        {
            monster.StateMachine.SetState(MonStateType.Idle);
        }
    }
    
    public void Exit(Monster monster)
    {
        // 상태가 종료될 때 동작 구현
        monster.animator.SetBool("isMove", false);
        if(monster.moveType == MoveType.Fly)
        {
            monster.moveSpeed/=2;
        }
    }
}