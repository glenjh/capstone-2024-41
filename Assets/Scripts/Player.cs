using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum PlayerStates
{
    Idle,
    Move,
    Jump,
    Attack,
    Dead
}

public class Player : MonoBehaviour
{
    public Rigidbody2D rigid;
    public PlayerStates PS;
    public PlayerState _state;
    public Animator anim;
    public GameObject playerFeet;
    public SpriteRenderer sprite;

    public Vector2 move;
    public Transform pos;
    public Vector2 boxSize;
    public float jumpForce = 5;
    public int life = 3;
    public bool isHit;
    public bool isAtk = false;
    private float horizontalMove;
    private float jumpTimer = 0f;
    private float jumpTimerMax = 1f;

    void Init()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerFeet = GetComponent<GameObject>();
        sprite = GetComponent<SpriteRenderer>();
        _state = new Idle();

        isHit = false;
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        if (PS == PlayerStates.Dead)
        {
            return;
        }
        _state.Update(this);
    }

    public void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Enemy" && !isHit)
        {
            if (life > 1)
            {
                anim.SetTrigger("doDamage");
                life--;
                StartCoroutine(OnDamage());
            }
            else
            {
                PS = PlayerStates.Dead;
                HandleInput();
                return;
            }
        }
    }

    IEnumerator OnDamage()
    {
        rigid.AddForce(new Vector2(Math.Sign(rigid.velocity.x * -20), jumpForce * 1.5f), ForceMode2D.Impulse);
        sprite.color = new Color(1, 1, 1, 0.4f);
        isHit = true;
        
        yield return new WaitForSeconds(0.5f);
        
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

    // void StateUpdate()
    // {
    //     Idle();
    //     Attack();
    //     Move();
    //     Jump();
    //     Death();
    // }

    public void Idle()
    {
        PS = PlayerStates.Idle;
        HandleInput();
    }
    
    public void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            isAtk = true;
            Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(pos.position, boxSize, 0);
            foreach (Collider2D collider in collider2Ds)
            {
                if (collider.tag == "Enemy")
                {
                    collider.GetComponent<Dummy>().TakeDamege();
                    // Debug.Log("ATTACK");
                }
            }
            PS = PlayerStates.Attack;
            HandleInput();
        }
    }

    public void OnDrawGizmos() // 피격범위를 보기위함 주석처리 요망
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(pos.position, boxSize);
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

    public void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) )
        {
            PS = PlayerStates.Jump;
            HandleInput();
            rigid.velocity = new Vector2(rigid.velocity.x, jumpForce);
        }
    }
}