using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public enum PlayerStates
{
    Idle,
    Move,
    Jump,
    Attack,
    JumpAttack,
    Stamping,
    Dead
}

public class Player : MonoBehaviour, IDamageAble
{
    [Header ("Player Setting")]
    public Rigidbody2D rigid;
    public PlayerStates PS;
    public PlayerState _state;
    public Animator anim;
    public GameObject playerFeet;
    public SpriteRenderer sprite;
    public int life = 10;
    public int maxLife = 10;

    public int Life => life;

    public bool isHit;
    public float normalGravity = 1f;
    private float horizontalMove;
    
    public bool isGround = true;
    
    [Header ("Player Move")]
    public Vector2 move;
    
    [Header ("Jump")]
    public float jumpForce = 7;
    public float coyoteTime = 0.2f;
    public float coyoteTimeCounter;
    public float jumpBufferingTime = 0.2f;
    public float jumpBufferingCounter;
    
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
    private bool pulseUnlocked = false;
    public float pulseAttackCoolDown = 1f;
    private float pulseAttackTime = 0.5f;
    
    [Header ("Dash")]
    public GameObject dashEffect;
    public Transform dashEffectPos;
    public bool canDash = true;
    public bool isDashing;
    public float dashSpeed = 10f;
    private float dashingTime = 0.3f;
    private float dashCoolDown = 1f;
    
    [Header ("Layer Ignore")]
    public bool fallFromHoverGround;
    private int hoverGroundLayer, playerLayer;

    [Header("Player Hit")] 
    public GameObject hitEffect;
    public Transform hitEffectPos;
    

    void Init()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerFeet = GetComponent<GameObject>();
        sprite = GetComponent<SpriteRenderer>();
        _state = new Idle();

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
         if (Input.GetKeyDown(KeyCode.Q))
         {
             if (pulseUnlocked && pulseAttackAble)
             {
                 StartCoroutine("PulseAttack");
             }
             else
             {
                 Debug.Log("locked!");
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
        
        if (PS == PlayerStates.Dead)
        {
            return;
        }
        _state.Update(this);
        
        
        if (rigid.velocity.y > 0)
        {
            IgnoreLayerON();
        }
        else if(rigid.velocity.y <= 0 && !fallFromHoverGround)
        {
            IgnoreLayerOFF();
        }
    }

    void FallingAnim()
    {
        if (rigid.velocity.y < -0.2f)
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
            yield return new WaitForSecondsRealtime(0.6f);
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
            StartCoroutine(OnDamage());
        }
        
        if(life < 1)
        {
            PS = PlayerStates.Dead;
            HandleInput();
            return;
        }
    }

    IEnumerator OnDamage()
    {
        sprite.color = new Color(1, 1, 1, 0.4f);
        isHit = true;
        
        yield return new WaitForSeconds(1f);
        
        sprite.color = new Color(1, 1, 1, 1);
        isHit = false;
    }

    public void HandleInput()
    {
        PlayerState temp = _state.HandleInput(PS);

        if (temp != null)
        {
            _state.Exit(this);
            _state = temp;
            _state.Enter(this);
        }
    }

    public void Idle()
    {
        PS = PlayerStates.Idle;
        HandleInput();
    }

    public void OnAttack()
    {
        playerAttackCollider.enabled = true;
    }

    public void OffAttack()
    {
        playerAttackCollider.enabled = false;
    }
    
    public void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            PS = PlayerStates.Attack;
            HandleInput();
        }
    }
    
    public void JumpAttack()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            PS = PlayerStates.JumpAttack;
            HandleInput();
        }
    }

    public void HalfGravity()
    {
        rigid.gravityScale = 0.5f;
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
            if (PS != PlayerStates.Move)
            {
                PS = PlayerStates.Move;
                HandleInput();
            }
        }
        else
        {
            if(PS != PlayerStates.Idle)
            {
                Idle();
            }
        }
    }

    public void Move()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
        move = new Vector2(horizontalMove * 5, rigid.velocity.y);
        
        rigid.velocity = new Vector2(move.x, rigid.velocity.y);
        if (rigid.velocity.x != 0)
        {
            transform.localScale = new Vector2(Math.Sign(rigid.velocity.x), 1);
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
        if (Input.GetButtonDown("Jump"))
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
        if (coyoteTimeCounter > 0f && jumpBufferingCounter > 0f )
        {
            jumpBufferingCounter = 0f;
            PS = PlayerStates.Jump;
            HandleInput();
        }
    }
    
    public void ReturnJump()
    {
        PS = PlayerStates.Jump;
        HandleInput();
    }

    public void Stamp()
    {
        if (Input.GetKey(KeyCode.DownArrow))
        {
            PS = PlayerStates.Stamping;
            HandleInput();
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
            
            if (collider.gameObject.tag == "Enemy" && isGround && PS == PlayerStates.Stamping)
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

   

    public IEnumerator Dash()
    {
        GameObject newDashEffect = Instantiate(dashEffect, dashEffectPos.position, Quaternion.identity);
        newDashEffect.transform.localScale = transform.localScale * 0.5f;

        anim.SetBool("isDashing", true);
        isDashing = true;
        canDash = false;
        rigid.gravityScale = 0;
        rigid.velocity = new Vector2(transform.localScale.x * dashSpeed, 0);
        isHit = true;
        yield return new WaitForSecondsRealtime(dashingTime);
        isHit = false;
        anim.SetBool("isDashing", false);

        rigid.gravityScale = normalGravity;
        isDashing = false;
        rigid.velocity = Vector2.zero;
        yield return new WaitForSecondsRealtime(dashCoolDown);

        canDash = true;
    }

    // public void SpecialAttacking()
    // {
    //     if (Input.GetKeyDown(KeyCode.Q))
    //     {
    //         PS = PlayerStates.SpecialAttack;
    //         HandleInput();
    //     }
    // }
    

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Enemy") || col.gameObject.CompareTag("Boss"))
        {
            TakeHit(1,null);
        }
    }
}