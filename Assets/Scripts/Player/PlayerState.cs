using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;

public class PlayerState
{
    public PlayerState() {}
    
    public virtual  void Enter(Player player) {}
    
    public virtual  void Exit(Player player) {}
    
    public virtual  void Update(Player player) {}

    public virtual PlayerState HandleInput(PlayerStates ps)
    {
        if (ps != null)
        {
            switch (ps)
            {
                case PlayerStates.Idle:
                    return new Idle(ps);
                
                case PlayerStates.Move:
                    return new Move(ps);
                
                case PlayerStates.Jump:
                    return new Jump(ps);
                
                case PlayerStates.Attack:
                    return new Attack(ps);
                
                case PlayerStates.JumpAttack:
                    return new JumpAttack(ps);

                case PlayerStates.Stamping:
                    return new Stamping(ps);
                
                case PlayerStates.Dead:
                    return new Dead(ps);
                
                default:
                    return null;
            }
        }

        return null;
    }
}

public class Idle : PlayerState
{
    public Idle() {}

    public Idle(PlayerStates ps)
    {
        ps = PlayerStates.Idle;
    }

    public override void Enter(Player player)
    {
        if (!player.isDashing)
        {
            player.rigid.velocity = new Vector2(0, player.rigid.velocity.y);
        }
        player.anim.SetBool("isMoving", false);
    }

    public override void Update(Player player)
    {
        player.StartCoroutine("LayerOpenClose");
        if (player.canDash && Input.GetKeyDown(KeyCode.C))
        {
            player.StartCoroutine("Dash");
        }

        if (!player.isDashing)
        {
            player.SetMoveState();
            player.Attack();
            player.Jump();
        }
    }

    public override void Exit(Player player)
    {
    }

    public override PlayerState HandleInput(PlayerStates ps)
    {
        return base.HandleInput(ps);
    }
}

public class Move : PlayerState
{
    public Move() {}

    public Move(PlayerStates ps)
    {
        ps = PlayerStates.Move;
    }

    public override void Enter(Player player)
    {
        player.anim.SetBool("isMoving", true);
    }

    public override void Update(Player player)
    {

        player.StartCoroutine("LayerOpenClose");
        
        if (player.canDash && Input.GetKeyDown(KeyCode.C))
        {
            player.StartCoroutine("Dash");
        }

        if(!player.isDashing)
        {
            player.Move();
            player.SetMoveState();
            player.Jump();
            player.Stamp();
        }
    }

    public override void Exit(Player player)
    {
        player.anim.SetBool("isMoving", false);
    }

    public override PlayerState HandleInput(PlayerStates ps)
    {
        return base.HandleInput(ps);
    }
}

public class Jump : PlayerState
{
    public Jump() {}

    public Jump(PlayerStates ps)
    {
        ps = PlayerStates.Jump;
    }

    public override void Enter(Player player)
    {
        player.rigid.velocity = new Vector2(player.rigid.velocity.x, player.jumpForce);
        player.anim.SetBool("isJumping", true);
    }

    public override void Update(Player player)
    {
        if (Input.GetButtonUp("Jump") && player.rigid.velocity.y > 0f)
        {
            player.rigid.velocity = new Vector2(player.rigid.velocity.x, player.rigid.velocity.y * 0.5f);
            player.coyoteTimeCounter = 0f;
        }
        
        if (player.canDash && Input.GetKeyDown(KeyCode.C))
        {
            player.StartCoroutine("Dash");
        }

        if (!player.isDashing)
        {
            player.Move();
            player.JumpAttack();
            player.Stamp();
        }
    }

    public override void Exit(Player player)
    {
        player.anim.SetBool("isJumping", false);
    }

    public override PlayerState HandleInput(PlayerStates ps)
    {
        return base.HandleInput(ps);
    }
}

public class Attack : PlayerState
{
    private float attackMoveSpeed = 1.0f;
    public Attack() {}

    public Attack(PlayerStates ps)
    {
        ps = PlayerStates.Attack;
    }

    public override void Enter(Player player)
    {
        player.anim.SetTrigger("doAttack");
        player.rigid.velocity = new Vector2(player.transform.localScale.x * attackMoveSpeed, player.rigid.velocity.y);
    }

    public override void Update(Player player)
    {
        player.Attack();
    }

    public override void Exit(Player player)
    {
        
    }

    public override PlayerState HandleInput(PlayerStates ps)
    {
        return base.HandleInput(ps);
    }
}

public class JumpAttack : PlayerState
{
    public JumpAttack() {}

    public JumpAttack(PlayerStates ps)
    {
        ps = PlayerStates.JumpAttack;
    }

    public override void Enter(Player player)
    {
        player.anim.SetTrigger("doJumpAttack");
        player.rigid.velocity = new Vector2(0, 0);
    }

    public override void Update(Player player)
    {
        player.JumpAttack();
        player.Stamp();
    }

    public override void Exit(Player player)
    {
    }

    public override PlayerState HandleInput(PlayerStates ps)
    {
        return base.HandleInput(ps);
    }
}

public class Stamping : PlayerState
{
    public Stamping() {}

    public Stamping(PlayerStates ps)
    {
        ps = PlayerStates.Stamping;
    }

    public override void Enter(Player player)
    {
        player.rigid.AddForce(Vector2.down * player.stampingPower);
    }

    public override void Update(Player player)
    {
    }
    
    public override void Exit(Player player) {}
    
    public override PlayerState HandleInput(PlayerStates ps)
    {
        return base.HandleInput(ps);
    }
}

public class Dead : PlayerState
{
    public Dead() {}

    public Dead(PlayerStates ps)
    {
        ps = PlayerStates.Dead;
    }

    public override void Enter(Player player)
    {
        player.anim.SetTrigger("isDead");
        player.rigid.velocity = new Vector2(0, player.rigid.velocity.y);
        player.rigid.constraints = RigidbodyConstraints2D.FreezeAll;
    }
    
    public override void Update(Player player) {}
    
    public override void Exit(Player player) {}

    public override PlayerState HandleInput(PlayerStates ps)
    {
        return base.HandleInput(ps);
    }
}
