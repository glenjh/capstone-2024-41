using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class AncientBoss : Boss
{
    [SerializeField] private GameObject _thunder;
    [SerializeField] private GameObject _rangeAttack;
    [SerializeField] private GameObject _arms;
    [SerializeField] private GameObject _body;
    public Action attackAction;
    
    bool isBuffed = false;
    
    // 근접 공격 델리게이트 인스턴스
    private List<Action> attackActions = new List<Action>();

    protected override void Start()
    {
        base.Start();
        
        StateMachine.SetState(BossStateType.Sleep);

        // 각 델리게이트에 메서드 연결
        attackActions.Add(MeleeAttack);
        attackActions.Add(RangeAttack);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if(Input.GetKeyDown(KeyCode.M))
            WakeUp();
    }

    public override void OnAttack()
    {
        var m = Vector2.Distance(transform.position, player.transform.position);
        
        //리스트내에 있는 함수를 임의로 실행
        attackActions[UnityEngine.Random.Range(0, attackActions.Count)]();
    }
    
    public override void TakeHit(int damage, Transform transform)
    {
        base.TakeHit(damage, transform);
        if(!isBuffed&&health<=maxHealth/2)
        {
            isBuffed = true;
            StateMachine.SetState(BossStateType.Buff);
        }
    }

    public override void OnBuff()
    {
        attackActions[1] = BuffRangeAttack;
        attackActions.Add(SpinChargeAttack);
        StartCoroutine(BuffColorChange(this));
    }

    // 각 공격 패턴에 해당하는 메서드 정의
    private void MeleeAttack()
    {
        // 근접 공격 1 애니메이션 재생 및 공격 로직 실행
        animator.SetTrigger("DoMeleeAttack");
        // ...
    }

    private void BuffRangeAttack()
    {
        // 페이즈2 원거리 공격 2 애니메이션 재생 및 공격 로직 실행
        animator.SetTrigger("DoRangeAttack2");
        // ...
    }
    
    private void SpinChargeAttack()
    {
        // 페이즈2 회전 공격 애니메이션 재생 및 공격 로직 실행
        animator.SetTrigger("DoSpinChargeAttack");
        // ...
    }

    private void RangeAttack()
    {
        // 원거리 공격 1 애니메이션 재생 및 공격 로직 실행
        animator.SetTrigger("DoRangeAttack");
        // ...
    }
    
    public void Buff()
    {
        animator.SetTrigger("DoBuff");
    }
    
    public void OnThunder()
    {
        _thunder.SetActive(true);
        var playerPos = player.transform.position;
        _thunder.transform.position = new Vector3(playerPos.x, transform.position.y + 1f, transform.position.z);
    }

    public void OnRange()
    {
        _rangeAttack.SetActive(true);
    }
    
    public void OnSpinAttack()
    {
        print("SpinAttack");
        _arms.SetActive(true);
        _arms.transform.position = this.transform.position;
        _body.SetActive(true);
        spriteRenderer.color = new Color(0, 0, 0, 0f);
        isSuperArmor = true;
    }
    
    public void OffRangeAttack()
    {
        _rangeAttack.SetActive(false);
    }
    //버프시 서서히 색 변화 코루틴
    IEnumerator BuffColorChange(Boss boss)
    {
        Color startColor = Color.white; // 시작 색상 (백그라운드 색상)
        Color targetColor = new Color(1f, 0.6f, 0.4f); // 목표 색상 (빨간색)
        _thunder.GetComponent<SpriteRenderer>().color = targetColor;
        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime;
            boss.spriteRenderer.color = Color.Lerp(startColor, targetColor, time);
            yield return null;
        }
    }

    public override void OffAttack()
    {
        base.OffAttack();
        OffSpinAttack();
    }

    public void OffSpinAttack()
    {
        _arms.SetActive(false);
        _body.SetActive(false);
        animator.SetBool("isDead",true);
        isSuperArmor = false;
        spriteRenderer.color = new Color(1f, 0.6f, 0.4f);
    }
    
    public override void Dead()
    {
        OffAttack();
        //attackAction();
        base.Dead();
    }
}