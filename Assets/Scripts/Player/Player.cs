using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public enum PlayerStates
{
    IDLE,
    MOVE,
    JUMP,
    ATTACK,
    JUMPATTACK,
    STAMPING,
    DASH,
    PARRYING,
    WALLSLIDING,
    WALLJUMPING,
    DEAD
}

public class Player : MonoBehaviour, IDamageAble
{
    [Header ("Player Setting")]
    public Rigidbody2D rigid;
    public PlayerStates PS;
    public PlayerStateMachine _stateMachine;
    public Animator anim;
    public GameObject playerFeet;
    public SpriteRenderer sprite;
    public ParticleSystem walkingDust;
    public int life = 10;
    public int maxLife = 10;

    public bool isHit;
    public float normalGravity = 1f;
    private float horizontalMove;
    public Ghost ghost;

    public bool isGround = true;
    
    [Header ("Player Move")]
    public Vector2 move;
    
    [Header ("Jump")]
    public float jumpForce = 7;
    public float coyoteTime = 0.2f;
    public float coyoteTimeCounter;
    public float jumpBufferingTime = 0.2f;
    public float jumpBufferingCounter;
    public ParticleSystem jumpAndLandDust;
    
    [Header ("Attack")]
    public Transform pos;
    public Vector2 boxSize;
    [SerializeField] private Collider2D playerAttackCollider;
    
    [Header ("Stamp Attack")]
    public GameObject stampEffect;
    public Transform stampEffectPos;
    public float stampingPower = 100;
    public Transform stampPos;
    public Vector2 stampBoxSize;

    [Header("Pulse Attack")] 
    public GameObject pulseEffect;
    public Transform pulseEffectPos;
    public bool pulseAttackAble = true;
    public bool isPulseAttacking;
    public bool pulseUnlocked = false;
    public float pulseAttackCoolDown = 1f;
    private float pulseAttackTime = 0.5f;
    
    [Header ("Dash")]
    public GameObject dashEffect;
    public Transform dashEffectPos;
    public bool canDash = true;
    public bool isDashing;
    public float dashSpeed = 10f;
    private float dashingTime = 0.4f;
    private float dashCoolDown = 1f;

    [Header(("Parrying"))] 
    [SerializeField] public Collider2D parryingCol;
    public bool canParrying = true;
    public bool isParrying;
    private float parryingTime = 3f;
    private float parryingCoolDown = 1f;


    [Header ("Layer Ignore")]
    public bool fallFromHoverGround;
    private int hoverGroundLayer, playerLayer;

    [Header("Player Hit")] 
    public GameObject hitEffect;
    public Transform hitEffectPos;

    [Header("WallJump")] 
    public ParticleSystem slideDust;
    public Transform wallChk;
    public float wallChkDistance;
    public float slideSpeed;
    public float wallJumpPower;
    public bool isWall;
    public bool isWallSliding = false;
    public bool isWallJumping;
    public LayerMask wallLayer;

    CinemachineImpulseSource impulseSource;

    void Init()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerFeet = GetComponent<GameObject>();
        sprite = GetComponent<SpriteRenderer>();
        _stateMachine = new PlayerStateMachine(PlayerStates.IDLE, this);
        _stateMachine.AddState(PlayerStates.MOVE);
        _stateMachine.AddState(PlayerStates.JUMP);
        _stateMachine.AddState(PlayerStates.ATTACK);
        _stateMachine.AddState(PlayerStates.JUMPATTACK);
        _stateMachine.AddState(PlayerStates.STAMPING);
        _stateMachine.AddState(PlayerStates.DASH);
        _stateMachine.AddState(PlayerStates.DEAD);
        
        isHit = false;

        hoverGroundLayer = LayerMask.NameToLayer("HoverGround");
        playerLayer = LayerMask.NameToLayer("Player");
        
    }

    void Start()
    {
        Init();
    }

     void OnEnable()
     {
         AchiveManager.onBossDefeated += HandleBossDefeated;
     }

     void OnDisable()
     {
         AchiveManager.onBossDefeated -= HandleBossDefeated;
     }

     private void HandleBossDefeated(string bossType)
     {
         switch (bossType)
         {
             case "Dummy":
                 pulseUnlocked = true;
                 break;
         }
     }

     public void UsePulseAttack()
     {
         if (!pulseUnlocked)
             return;
         if (Input.GetKeyDown(KeyCode.Q))
         {
             if (pulseAttackAble)
             {
                 StartCoroutine("PulseAttack");
             }
         }
     }
     
     public IEnumerator PulseAttack()
     {
        
         GameObject newPulseEffect = Instantiate(pulseEffect, pulseEffectPos.position, Quaternion.identity);
         newPulseEffect.transform.localScale = transform.localScale * 7f;

         isPulseAttacking = true;
         pulseAttackAble = false;
         rigid.velocity = new Vector2(0, 0);
         yield return new WaitForSecondsRealtime(pulseAttackTime);
        
         isPulseAttacking = false;
         yield return new WaitForSecondsRealtime(pulseAttackCoolDown);

         pulseAttackAble = true;
     }

    void Update()
    {
        UsePulseAttack();
        
        JumpBufferCount();
        CoyoteCount();
        
        FallingAnim();

        if (PS == PlayerStates.DEAD)
        {
            return;
        }
        
        _stateMachine.Action();


        if (rigid.velocity.y > 0)
        {
            IgnoreLayerON();
        }
        else if(rigid.velocity.y <= 0 && !fallFromHoverGround)
        {
            IgnoreLayerOFF();
        }

        isWall = Physics2D.Raycast(wallChk.position,
                 Vector2.right * transform.localScale.x,
                 wallChkDistance, wallLayer);

        Debug.DrawRay(wallChk.position, Vector2.right * transform.localScale.x * wallChkDistance, Color.cyan);
    }

    public void SetWallsliding()
    {
        if (isWall && !isGround && horizontalMove != 0)
        {
            _stateMachine.ChangeState(PlayerStates.WALLSLIDING);
        }
    }

    public void ExitWallSliding()
    {
        if (_stateMachine.GetCurrState().stateType != PlayerStates.WALLSLIDING)
        {
            return;
        }
        
        if ((!(Input.GetAxisRaw("Horizontal") == transform.localScale.x || Input.GetAxisRaw("Horizontal") == 0)) || !isWall)
        {
            SetMoveState();
        }
    }

    public void SetWalljumping()
    {
        if (Input.GetAxis("Jump") != 0)
        {
            _stateMachine.ChangeState(PlayerStates.WALLJUMPING);
        }
    }

    public void WallSlide()
    {
        if (isWall && !isGround && horizontalMove != 0)
        {
            ParticleON(slideDust);
            isWallJumping = false;
            isWallSliding = true;
            // anim.SetBool("isJumping", false);
            anim.SetBool("wallSliding", true);
            rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y * slideSpeed);
            WallJump();
        }
        else
        {
            isWallSliding = false;
            anim.SetBool("wallSliding", false);
        }
    }

    public void WallJump()
    {
        if (Input.GetAxis("Jump") != 0 && isWallSliding)
        {
            // isWallJumping = true;
            // anim.SetBool("isJumping", true);
            // anim.SetBool("isFalling", false);
            Invoke("MoveLock", 0.3f);
            rigid.velocity = new Vector2(-transform.localScale.x * wallJumpPower * 0.7f, wallJumpPower);
            transform.localScale = new Vector2(-Mathf.Sign(transform.localScale.x), 1);
        }
    }

    public void MoveLock()
    {
        isWallJumping = false;
    }
    public void ParticleON(ParticleSystem particleName)
    {
        particleName.Play();
    }

    void FallingAnim()
    {
        if (rigid.velocity.y < -0.6f)
        {
            anim.SetBool("isFalling", true);
        }
        else
        {
            anim.SetBool("isFalling", false);
        }
    }

    public void IgnoreLayerON()
    {
        Physics2D.IgnoreLayerCollision(playerLayer, hoverGroundLayer, true);
    }

    public void IgnoreLayerOFF()
    {
        Physics2D.IgnoreLayerCollision(playerLayer, hoverGroundLayer, false);
    }

    public IEnumerator LayerOpenClose()
    {
        if (Input.GetKey(KeyCode.S))
        {
            fallFromHoverGround = true;
            IgnoreLayerON();
            yield return new WaitForSecondsRealtime(0.3f);
            IgnoreLayerOFF();
            fallFromHoverGround = false;
        }
    }

    public void TakeHit(int damage, Transform other)
    {
        if (life >= 1)
        {
            GameObject newHitEffect = Instantiate(hitEffect, hitEffectPos.position, Quaternion.identity);
            newHitEffect.transform.localScale = transform.localScale;
            anim.SetTrigger("doDamage");
            life--;
            StartCoroutine(OnDamage(1f));
        }
        
        if(life < 1)
        {
            PS = PlayerStates.DEAD;
            _stateMachine.Action();
            return;
        }
    }

    public IEnumerator OnDamage(float invincibleTime)
    {
        sprite.color = new Color(1, 1, 1, 0.9f);
        isHit = true;
        
        yield return new WaitForSeconds(invincibleTime);
        
        sprite.color = new Color(1, 1, 1, 1);
        isHit = false;
    }

    public void Idle()
    {
        _stateMachine.ChangeState(PlayerStates.IDLE);
    }

    public void OnAttack()
    {
        playerAttackCollider.enabled = true;
    }

    public void OffAttack()
    {
        playerAttackCollider.enabled = false;
    }

    public void TirggerReset()
    {
        anim.ResetTrigger("doAttack");
        anim.ResetTrigger("doJumpAttack");
    }
    
    public void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Z) && !isWallSliding)
        {
            _stateMachine.ChangeState(PlayerStates.ATTACK);
        }
    }

    public void JumpAttack()
    {
        if (Input.GetKeyDown(KeyCode.Z) && !isWallSliding)
        {
            _stateMachine.ChangeState(PlayerStates.JUMPATTACK);
        }
    }

    public void HalfGravity()
    {
        rigid.gravityScale = 1f;
    }

    public void NormalGravity()
    {
        rigid.gravityScale = normalGravity;
    }

    public void SetMoveState()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");

        if (horizontalMove != 0)
        {
            if (PS != PlayerStates.MOVE)
            {
                _stateMachine.ChangeState(PlayerStates.MOVE);
            }
        }
        else
        {
            if(PS != PlayerStates.IDLE && PS != PlayerStates.DASH && PS != PlayerStates.PARRYING)
            {
                _stateMachine.ChangeState(PlayerStates.IDLE);
            }
        }
    }

    public void Move()
    {
        if (!anim.GetBool("canMove"))
        {
            return;
        }
        if (!isWallJumping)
        {
            horizontalMove = Input.GetAxisRaw("Horizontal");
            move = new Vector2(horizontalMove * 7, rigid.velocity.y);

            rigid.velocity = new Vector2(move.x, rigid.velocity.y);
            if (rigid.velocity.x != 0)
            {
                transform.localScale = new Vector2(Math.Sign(rigid.velocity.x), 1);
            }
        }
    }

    public void CoyoteCount()
    {
        if (isGround)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }
    
    public void JumpBufferCount()
    {
        if (Input.GetButtonDown("Jump") && anim.GetBool("canMove"))
        {
            jumpBufferingCounter = jumpBufferingTime;
        }
        else
        {
            jumpBufferingCounter -= Time.deltaTime;
        }
    }
    
    public void Jump()
    {
        if (coyoteTimeCounter > 0f && jumpBufferingCounter > 0f)
        {
            jumpBufferingCounter = 0f;
            _stateMachine.ChangeState(PlayerStates.JUMP);
        }
    }
    
    public void ReturnJump()
    {
        _stateMachine.ChangeState(PlayerStates.JUMP);
    }

    public void Stamp()
    {
        if (Input.GetKey(KeyCode.DownArrow) && anim.GetBool("isFalling") && !isWallSliding)
        {
            _stateMachine.ChangeState(PlayerStates.STAMPING);
        }
    }

    public void StampAttack()
    {
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(stampPos.position, stampBoxSize, 0);
        foreach (Collider2D collider in collider2Ds)
        {
            if (collider.gameObject.tag == "Ground")
            {
                GameObject newStampEffect = Instantiate(stampEffect, stampEffectPos.position, Quaternion.identity);
                newStampEffect.transform.localScale = transform.localScale * 0.3f;
            }
            
            if (collider.gameObject.tag == "Enemy" && isGround && PS == PlayerStates.STAMPING)
            {
                collider.GetComponent<IDamageAble>()?.TakeHit(1,null);
            }
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(stampPos.position, stampBoxSize);
    }

    public void SetDash()
    {
        if (Input.GetKeyDown(KeyCode.C) && canDash)
        {
            _stateMachine.ChangeState(PlayerStates.DASH);
        }
    }
    
    public IEnumerator Dash()
    {
        GameObject newDashEffect = Instantiate(dashEffect, dashEffectPos.position, Quaternion.identity);
        newDashEffect.transform.localScale = transform.localScale * 0.5f;
        
        canDash = false;
        rigid.gravityScale = 0;
        rigid.velocity = new Vector2(transform.localScale.x * dashSpeed, 0);
        isHit = true;
        yield return new WaitForSecondsRealtime(dashingTime);
        _stateMachine.ChangeState(PlayerStates.IDLE);
        
        isHit = false;
        rigid.gravityScale = normalGravity;
        yield return new WaitForSecondsRealtime(dashCoolDown);

        canDash = true;
    }

    public void SetParrying()
    {
        if (Input.GetKeyDown(KeyCode.X) && canParrying)
        {
            _stateMachine.ChangeState(PlayerStates.PARRYING);
        }
    }

    public IEnumerator ParryingStart()
    {
        isParrying = true;
        canParrying = false;
        anim.SetBool("isParrying", true);
        anim.SetBool("canMove", false);
        parryingCol.enabled = true;
        yield return new WaitForSecondsRealtime(parryingTime);
        ParryingSuccess();
    }

    public void ParryingSuccess()
    {
        anim.SetBool("isParrying", false);
        anim.SetBool("canMove", true);
        parryingCol.enabled = false;
        isParrying = false;

        StartCoroutine(ParryingCoolDown());
    }

    public IEnumerator ParryingCoolDown()
    {
        yield return new WaitForSecondsRealtime(parryingCoolDown);

        canParrying = true;
    }
    
    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Enemy") || col.gameObject.CompareTag("Boss"))
        {
            TakeHit(1,null);
        }
    }
}