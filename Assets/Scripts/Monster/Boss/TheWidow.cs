using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;

public class TheWidow : Boss {
    [SerializeField] private GameObject _meleeAttack;
    [SerializeField] private GameObject _rangeAttack;
    [SerializeField] private GameObject _superMeleeAttack;
    [SerializeField] private GameObject _superRangeAttack;
    [SerializeField] private GameObject _landAttack;
    [SerializeField] private GameObject _webAttack;
    [SerializeField] private GameObject _warningBox;
    [SerializeField] private GameObject _bezierMissileShooter;

    [SerializeField] private float leftSide;
    [SerializeField] private float rightSide;
    [SerializeField] private float parentX;
    bool isBuffed = false;
    
    int attackIndex = 0;
    
    private List<Tuple<float,Action>> attackActions = new List<Tuple<float,Action>>();
    private float curAttackLength = 0;
        
    private static readonly int Land = Animator.StringToHash("DoLand");

    protected override void Start() {
        base.Start();
        StateMachine.SetState(BossStateType.Sleep);
        
        attackActions.Add(new Tuple<float, Action>(10f, MeleeAttack));
        attackActions.Add(new Tuple<float, Action>(10f, RangeAttack));
        attackActions.Add(new Tuple<float, Action>(0f, BezierMissileAttack));
        attackActions.Add(new Tuple<float, Action>(0f, JumpAttack));
        
        minAttackRange = attackActions[attackIndex].Item1;
    }

    protected override void Awake()
    {
        base.Awake();
        
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        if(Input.GetKeyDown(KeyCode.P))
            TakeHit(1000, player);
        base.FixedUpdate();
    }
    
    public override void OnBuff()
    {
        attackActions[0] = new Tuple<float, Action>(10f, SuperMeleeAttack);
        attackActions[1] = new Tuple<float, Action>(12f, SuperRangeAttack);
        _bezierMissileShooter.GetComponent<BezierMissileShooter>().m_shotCount = 12;
        StartCoroutine(BuffColorChange(this));
    }
    
    public void Buff()
    {
        animator.SetTrigger("DoBuff");
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

    public override void OnAttack()
    {
        curAttackLength = minAttackRange;
        attackActions[attackIndex].Item2();
        attackIndex = (attackIndex + 1) % attackActions.Count;
        minAttackRange = attackActions[attackIndex].Item1;
    }

    public void BackStep()
    {
        float dis = 0;
        if (transform.localScale.x > 0)
        {
            dis = player.position.x - curAttackLength;
            if (dis < leftSide+ parentX)
            {
                rb.MovePosition(new Vector2(leftSide + parentX, transform.position.y));
            }
            else
            {
                rb.MovePosition(new Vector2(player.position.x - curAttackLength, transform.position.y));
            }
        }
        else
        {
            dis = player.position.x + curAttackLength;
            if(dis > rightSide + parentX)
                rb.MovePosition(new Vector2(rightSide + parentX, transform.position.y));
            else
                rb.MovePosition(new Vector2(player.position.x + curAttackLength, transform.position.y));
        }
    }
    
    private void Turn()
    {
        bool _dirRight = transform.localScale.x > 0;
        bool isPlayerOnRight = player.position.x > transform.position.x;

        // 플레이어의 방향과 보스의 현재 방향이 다를 때만 방향 전환 처리
        if (_dirRight != isPlayerOnRight)
        {
            _dirRight = isPlayerOnRight;
            transform.localScale = new Vector3(-1*transform.localScale.x, transform.localScale.y, transform.localScale.z);
            
        }
    }

    #region Attack

    private void MeleeAttack()
    {
        Turn();
        var dis = Vector2.Distance(transform.position, player.transform.position);
        if(dis < curAttackLength)
            animator.SetTrigger("BackStep");
        animator.SetInteger("AttackType", 0);
    }
    
    private void RangeAttack()
    {
        Turn();
        var dis = Vector2.Distance(transform.position, player.transform.position);
        if(dis < curAttackLength)
            animator.SetTrigger("BackStep");
        animator.SetInteger("AttackType", 1);
    }
    
    private void JumpAttack()
    {
        animator.SetInteger("AttackType", 2);
        bossCollider.enabled = false;
        rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
    }
    
    private void SuperMeleeAttack()
    {
        Turn();
        var dis = Vector2.Distance(transform.position, player.transform.position);
        if(dis < curAttackLength)
            animator.SetTrigger("BackStep");
        animator.SetInteger("AttackType", 3);
    }
    
    private void SuperRangeAttack()
    {
        Turn();
        var dis = Vector2.Distance(transform.position, player.transform.position);
        if(dis < curAttackLength)
            animator.SetTrigger("BackStep");
        animator.SetInteger("AttackType", 4);
    }
    
    private void BezierMissileAttack()
    {
        animator.SetInteger("AttackType", 5);
    }

    public void JumpEnd()
    {
        spriteRenderer.color = new Color(0, 0, 0, 0);
        isSuperArmor = true;
        if (!isBuffed)
        {
            StartCoroutine(DrawWarning());
            _warningBox.SetActive(true);
        }
        else
        {
            StartCoroutine(WebAttack());
        }
    }
    
    IEnumerator WebAttack()
    {
        _webAttack.SetActive(true);
        yield return new WaitForSeconds(3f);
        _webAttack.SetActive(false);
        StartCoroutine(DrawWarning());
        _warningBox.SetActive(true);
    }

    IEnumerator DrawWarning()
    {
        float time = 0;
        float MaxTime = 1f;

        while (time < MaxTime)
        {
            time += Time.deltaTime;
            //플레이어의 x 좌표를 기준으로 워닝박스의 위치를 설정
            _warningBox.transform.position = new Vector3(player.position.x, 6.7f, _warningBox.transform.position.z);
            
            yield return null;
        }

        //1초간 대기
        yield return new WaitForSeconds(1f);
        _warningBox.SetActive(false);
        DoLand(_warningBox.transform);
    }

    IEnumerator BuffColorChange(Boss boss)
    {
        Color startColor = Color.white; // 시작 색상 (백그라운드 색상)
        Color targetColor = new Color(1f, 0.6f, 0.4f); // 목표 색상
        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime;
            boss.spriteRenderer.color = Color.Lerp(startColor, targetColor, time);
            yield return null;
        }
    }

    public void OnMelee()
    {
        _meleeAttack.SetActive(true);
    }
    
    public void OffMelee()
    {
        _meleeAttack.SetActive(false);
    }
    
    public void OnRange()
    {
        _rangeAttack.SetActive(true);
    }
    
    public void OffRange()
    {
        _rangeAttack.SetActive(false);
    }
    
    public void OnSuperMelee()
    {
        _superMeleeAttack.SetActive(true);
    }
    
    public void OffSuperMelee()
    {
        _superMeleeAttack.SetActive(false);
    }
    
    public void OnSuperRange()
    {
        _superRangeAttack.SetActive(true);
    }
    
    public void OffSuperRange()
    {
        _superRangeAttack.SetActive(false);
    }

    public void OnLand()
    {
        _landAttack.SetActive(true);
    }
    
    public void OffLand()
    {
        _landAttack.SetActive(false);
    }
    
    public void OnBezierMissile()
    {
        _bezierMissileShooter.GetComponent<BezierMissileShooter>().Fire();
    }

    public void DoLand(Transform target = null)
    {
        if (target != null)
        {
            float pos = target.position.x-parentX;
            if (pos < leftSide)
                rb.MovePosition(new Vector3(parentX + leftSide, transform.position.y, transform.position.z));
            else if (pos > rightSide)
                rb.MovePosition(new Vector3(parentX + rightSide, transform.position.y, transform.position.z));
            else
                rb.MovePosition(new Vector3(target.position.x, transform.position.y, transform.position.z));
        }
        if(isBuffed)
            spriteRenderer.color = new Color(1f, 0.6f, 0.4f); // 목표 색상
        else
            spriteRenderer.color = new Color(1, 1, 1, 1);
        isSuperArmor = false;
        bossCollider.enabled = true;
        _landAttack.SetActive(true);
        animator.SetTrigger(Land);
    }
    #endregion

}