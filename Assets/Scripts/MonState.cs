using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
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
        monster.animator.SetBool("isMove", false);
        monster.StartWait();
    }
    
    public void Action(Monster monster)
    {
    }
    
    public void Exit(Monster monster)
    {
        monster.StopWait();
    }
}

public class MonStateMove : IMonState
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
        // 다음 지형이 바닥인지 확인
        Vector2 frontVec = new Vector2(monster.rb.position.x + (monster.moveSpeed >0 ? 0.5f:-0.5f), monster.rb.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Ground"));
        if (rayHit.collider == null)
        {
            monster.moveSpeed *= -1;
            monster.transform.localScale = new Vector3(-1 * monster.transform.localScale.x, monster.transform.localScale.y, monster.transform.localScale.z); 
        }
        
        monster.rb.velocity = new Vector2(monster.moveSpeed, monster.rb.velocity.y);
    }

    //행동 결정 로직
    public void Decide(Monster monster)
    {
        // 몬스터가 이동할 방향 결정
        nextMove = Random.Range(0, 2);
        if (nextMove == 0)
        {
            monster.moveSpeed *= -1;
            monster.transform.localScale = new Vector3(-1 * monster.transform.localScale.x, monster.transform.localScale.y, monster.transform.localScale.z);
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
        monster.animator.SetTrigger("doDie");
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
    }
    
    public void Action(Monster monster)
    {
        if(target == null)
            return;
        
        if (target.position.x < monster.transform.position.x && monster.moveSpeed > 0)
        {
            monster.moveSpeed *= -1;
            monster.transform.localScale = new Vector3(-1 * monster.transform.localScale.x, monster.transform.localScale.y, monster.transform.localScale.z);
        }
        else if (target.position.x > monster.transform.position.x && monster.moveSpeed < 0)
        {
            monster.moveSpeed *= -1;
            monster.transform.localScale = new Vector3(-1 * monster.transform.localScale.x, monster.transform.localScale.y, monster.transform.localScale.z);
        }
        
        monster.rb.velocity = new Vector2(monster.moveSpeed, monster.rb.velocity.y);
    }
    
    public void Exit(Monster monster)
    {
        // 상태가 종료될 때 동작 구현
        monster.animator.SetBool("isMove", false);
    }
}