using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum PlayerStates
{
    IDLE,
    MOVE,
    JUMP,
    ATTACK,
    SLASHATTACK,
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
    [Header("Player Setting")] 
    public BoxCollider2D playerCol;
    public Rigidbody2D rigid;
    public PlayerStates PS;
    public PlayerStateMachine _stateMachine;
    public Animator anim;
    public GameObject playerFeet;
    public SpriteRenderer sprite;
    public ParticleSystem walkingDust;
    public int life = 10;
    public int maxLife = 10;
    [HideInInspector]
    public bool controlAble = true;
    public HitPauser _pauser;
    public ChangePostProcess sceneEffector;

    public bool isHit;
    public float normalGravity = 1f;
    private float horizontalMove;
    public Ghost ghost;

    public bool isGround = true;

    [Header("Player Move")] 
    public float moveSpeed = 7f;
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
    public float slashTime = 0.5f;
    public float slashPow = 30f;
    public float len = 10f;
    
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

    [Header("Layer Ignore")] 
    public GameObject hoverGround;

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

    [Header("Rage Mode")] 
    public bool isRage = false;
    [SerializeField] public TrailRenderer eyeLight;
    public float maxCharge = 100;
    public float currCharge = 0;
    public float rageSpeed = 8.5f;

    CinemachineImpulseSource impulseSource;
    [SerializeField]ShockWave shockWave;

    void Init()
    {
        playerCol = GetComponent<BoxCollider2D>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerFeet = GetComponent<GameObject>();
        sprite = GetComponent<SpriteRenderer>();
        
        _stateMachine = new PlayerStateMachine(PlayerStates.IDLE, this);
        _stateMachine.AddState(PlayerStates.MOVE);
        _stateMachine.AddState(PlayerStates.JUMP);
        _stateMachine.AddState(PlayerStates.ATTACK);
        _stateMachine.AddState(PlayerStates.SLASHATTACK);
        _stateMachine.AddState(PlayerStates.JUMPATTACK);
        _stateMachine.AddState(PlayerStates.STAMPING);
        _stateMachine.AddState(PlayerStates.DASH);
        _stateMachine.AddState(PlayerStates.DEAD);
        
        _stateMachine.AddState(PlayerStates.WALLSLIDING);
        _stateMachine.AddState(PlayerStates.WALLJUMPING);
        
        isHit = false;
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
        ZoomTest();
        
        UsePulseAttack();
        
        JumpBufferCount();
        CoyoteCount();
        
        FallingAnim();

        if (PS == PlayerStates.DEAD || !controlAble)
        {
            return;
        }
        
        _stateMachine.Action();
        
        isWall = Physics2D.Raycast(wallChk.position,
                 Vector2.right * transform.localScale.x,
                 wallChkDistance, wallLayer);

        Debug.DrawRay(wallChk.position, Vector2.right * transform.localScale.x * wallChkDistance, Color.cyan);
        Debug.DrawRay(transform.position, Vector2.right * -transform.localScale.x * len, Color.yellow);
    }

    void ZoomTest()
    {
        if(Input.GetKeyDown(KeyCode.R))
            CameraManager.instance.ChangeZoom(2f);
        if(Input.GetKeyDown(KeyCode.T))
            CameraManager.instance.ChangeZoom(-2f);
        
        if (Input.GetKeyDown(KeyCode.P))
            if (currCharge == maxCharge)
            {
                StartCoroutine(ChargeDown());
                sceneEffector.RageMode();
            }
        
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

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("HoverGround"))
        {
            hoverGround = collision.gameObject;
        }
    }

    public void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("HoverGround"))
        {
            hoverGround = null;
        }
    }

    public IEnumerator LayerOpenClose()
    {
        if (Input.GetKeyDown(KeyCode.S) && hoverGround != null)
        {
            CompositeCollider2D hoverCol = hoverGround.GetComponent<CompositeCollider2D>();
            
            Physics2D.IgnoreCollision(playerCol, hoverCol, true);
            yield return new WaitForSecondsRealtime(0.25f);
            Physics2D.IgnoreCollision(playerCol, hoverCol, false);
        }
    }

    public void TakeHit(int damage, Transform other)
    {
        if (life > 0 && !isHit)
        {
            GameObject newHitEffect = Instantiate(hitEffect, hitEffectPos.position, Quaternion.identity);
            newHitEffect.transform.localScale = transform.localScale;
            anim.SetTrigger("doDamage");
            life--;
            CameraManager.instance.CamShake();
            sceneEffector.ChromaticScreen();
            sceneEffector.VignetteScreen();
            sceneEffector.GlitchScreen();
            StartCoroutine(Freeze(0.35f));
            StartCoroutine(OnDamage(1f));
            _pauser.HitStop(0.2f); 
        }
        
        if(life <= 0)
        {
            StopAllCoroutines();
            PS = PlayerStates.DEAD;
            _stateMachine.ChangeState(PlayerStates.DEAD);
        }
    }

    public IEnumerator OnDamage(float invincibleTime)
    {
        sprite.color = new Color(1, 1, 1, 0.9f);
        isHit = true;
        
        yield return new WaitForSecondsRealtime(invincibleTime);
        
        sprite.color = new Color(1, 1, 1, 1);
        isHit = false;
    }

    public IEnumerator Freeze(float duration)
    {
        anim.SetBool("canMove", false);
        
        yield return new WaitForSecondsRealtime(duration);
        
        anim.SetBool("canMove", true);
    }

    public void Idle()
    {
        _stateMachine.ChangeState(PlayerStates.IDLE);
    }

    public void TriggerReset()
    {
        anim.ResetTrigger("doAttack");
        anim.ResetTrigger("doJumpAttack");
    }
    
    public void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Z) && !isWallSliding && isGround)
        {
            _stateMachine.ChangeState(PlayerStates.ATTACK);
        }
    }

    public void Slash()
    {
        if (Input.GetKeyDown(KeyCode.X) && !isWallSliding)
        {
            if (!isRage)
            {
                if (currCharge < 20f)
                {
                    return;
                }
                else
                {
                    currCharge -= 20;
                    _stateMachine.ChangeState(PlayerStates.SLASHATTACK);
                }
            }
            else if (isRage)
            {
                _stateMachine.ChangeState(PlayerStates.SLASHATTACK);
            }
        }
    }

    public void DoSalsh()
    {
        StartCoroutine("SlashMove");
    }

    public IEnumerator SlashMove()
    {
        rigid.velocity = new Vector2(transform.localScale.x * slashPow, 0);
        
        yield return new WaitForSecondsRealtime(slashTime);
        
        rigid.velocity = new Vector2(0, 0);
    }
    
    public void SlashRay()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.right * -transform.localScale.x * len);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.gameObject.CompareTag("Enemy") || hits[i].collider.gameObject.CompareTag("Boss"))
            {
                hits[i].collider.gameObject.GetComponent<IDamageAble>()?.TakeHit(1, null);
            }
        }
        shockWave.CallShockWave(transform.position);
    }

    public void JumpAttack()
    {
        if (Input.GetKeyDown(KeyCode.Z) && !isWallSliding && !isGround)
        {
            _stateMachine.ChangeState(PlayerStates.JUMPATTACK);
        }
    }

    public void Charge()
    {
        currCharge += 5;
        if (currCharge > maxCharge)
        {
            currCharge = maxCharge;
        }
    }

    public IEnumerator ChargeDown()
    {
        float elapsedTime = 0f;
        float duration = 5f;

        while (elapsedTime < duration)
        {
            currCharge = (Mathf.RoundToInt(Mathf.Lerp(maxCharge, 0f, elapsedTime / duration)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currCharge = 0;
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
            move = new Vector2(horizontalMove * moveSpeed, rigid.velocity.y);

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
        if (Input.GetKey(KeyCode.DownArrow) && !isGround && !isWallSliding)
        {
            _stateMachine.ChangeState(PlayerStates.STAMPING);
        }
    }

    public void StampAttack()
    {
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(stampPos.position, stampBoxSize, 0);
        CameraManager.instance.CamShake();
        foreach (Collider2D collider in collider2Ds)
        {
            if (collider.gameObject.tag == "Ground" || collider.gameObject.tag == "HoverGround")
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
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
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
        if (Input.GetKeyDown(KeyCode.C) && canParrying)
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
        if (isParrying)
        {
            ParryingSuccess();
        }
    }

    public void ParryingSuccess()
    {
        StopCoroutine("ParryingStart");
        shockWave.CallShockWave(transform.position);
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
    
    public void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Enemy") || col.gameObject.CompareTag("Boss"))
        {
            if (isHit||isParrying) return;
            CameraManager.instance.CamShake();
            TakeHit(1,null);
        }
    }
    
    public void EnablePlayerControlSignal()
    {
        controlAble = true;
    }

    public void DisablePlayerControlSignal()
    {
        controlAble = false;
    }

    public void FlipControlSignal()
    {
        transform.localScale = new Vector2(-transform.localScale.x, 1);
    }
}