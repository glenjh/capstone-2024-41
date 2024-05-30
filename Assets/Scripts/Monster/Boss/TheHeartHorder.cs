using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TheHeartHorder : Boss
{
    private List<Tuple<float,Action>> attackActions = new List<Tuple<float,Action>>();
    private float curAttackLength = 0;
    int attackIndex = 0;
    
    bool isMoveAttack = false;
    [SerializeField]private Bullet swordBullet;
    [SerializeField]private BezierMissileShooter bezierMissileShooter;
    
    protected override void Awake()
    {
        base.Awake();
    }
    
    protected override void Start()
    {
        base.Start();
        
        StateMachine.SetState(BossStateType.Sleep);
        
        attackActions.Add(new Tuple<float, Action>(10f, MeleeAttack));
        attackActions.Add(new Tuple<float, Action>(10f, SlamAttack));
        attackActions.Add(new Tuple<float, Action>(30f, MoveAttack));
        attackActions.Add(new Tuple<float, Action>(20f, RangeAttack));
        attackActions.Add(new Tuple<float, Action>(0f, BezireMissileAttack));
    }
    // Update is called once per frame
    void Update()
    {
        if (isMoveAttack)
        {
            bool _dirRight = transform.localScale.x > 0;
            rb.velocity = new Vector2(_dirRight ? moveSpeed * 4f : -moveSpeed * 4f, rb.velocity.y);

            MoveSlash();
        }
    }
    
    public override void OnAttack()
    {
        curAttackLength = minAttackRange;
        attackActions[attackIndex].Item2();
        attackIndex = (attackIndex + 1) % attackActions.Count;
        minAttackRange = attackActions[attackIndex].Item1;
    }
    
    #region Attack
    
    private void MeleeAttack()
    {
        animator.SetInteger("AttackType", 0);
    }
    private void SlamAttack()
    {
        animator.SetInteger("AttackType", 1);
    }

    private void MoveAttack()
    {
        animator.SetInteger("AttackType", 2);

        isMoveAttack = true;
    }

    private void MoveSlash()
    {
        var range = player.position.x - transform.position.x;
        if(Mathf.Abs(range) < 4)
        {
            animator.SetTrigger("DoSlash");
            isMoveAttack = false;
        }
    }
    
    private void RangeAttack()
    {
        animator.SetInteger("AttackType", 3);
    }
    
    public void OnRangeAttack()
    {
        //총알 좌우 발사
        var bullet = Instantiate(swordBullet.gameObject).GetComponent<Bullet>();
        bullet.transform.position = transform.position;
        bullet.Fire(Vector3.right, 15);
        
        
        bullet = Instantiate(swordBullet.gameObject).GetComponent<Bullet>();
        bullet.transform.position = transform.position;
        bullet.transform.localScale = new Vector3(-1,1,1);
        bullet.Fire(Vector3.left, 15);
    }
    
    private void BezireMissileAttack()
    {
        animator.SetInteger("AttackType", 4);
    }
    
    public void OnBezireMissileAttack()
    {
        //미사일 발사
        bezierMissileShooter.Fire();
    }
    
    #endregion
}
