using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using AncientBossAttak;

public class AncientBoss : Boss
{
    // 근접 공격 델리게이트 인스턴스
    private MeleeAttackDelegate meleeAttack;

    // 원거리 공격 델리게이트 인스턴스
    private RangedAttackDelegate rangedAttack;

    protected override void Start()
    {
        base.Start();

        // 각 델리게이트에 메서드 연결
        meleeAttack = MeleeAttack;
        rangedAttack = RangeAttack;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        
    }

    public override void OnAttack()
    {
        Debug.Log("OnAttack");
        MeleeAttack();
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
        animator.SetTrigger("MeleeAttack2");
        // ...
    }

    private void RangeAttack()
    {
        // 원거리 공격 1 애니메이션 재생 및 공격 로직 실행
        animator.SetTrigger("RangedAttack1");
        // ...
    }
}

namespace AncientBossAttak
{
    // 근접 공격 델리게이트
    public delegate void MeleeAttackDelegate();

    // 원거리 공격 델리게이트
    public delegate void RangedAttackDelegate();
}