using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;

public class PlayerState
{
    public PlayerStates stateType; 
    public PlayerState() {}
    
    public virtual  void Enter(Player player) {}
    
    public virtual  void Exit(Player player) {}
    
    public virtual  void Update(Player player) {}
}

public class Idle : PlayerState
{
    public Idle() {}

    public Idle(PlayerStates ps)
    {
        ps = PlayerStates.IDLE;
        stateType = ps;
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
        player.SetDash();
        player.SetWallsliding();

        if (player.rigid.velocity.y == 0)
        {
            player.SetParrying();
        }

        if (!player.isDashing && !player.isParrying)
        {
            player.SetMoveState();
            // if (player.rigid.velocity.y != 0)
            // {
            //     player.JumpAttack();
            // }
            // else
            // {
            //     player.Attack();
            // }
            player.JumpAttack();
            player.Attack();
            player.Jump();
            player.Stamp();
        }
    }

    public override void Exit(Player player)
    {
    }
}

public class Move : PlayerState
{
    public Move() {}

    public Move(PlayerStates ps)
    {
        ps = PlayerStates.MOVE;
        stateType = ps;
    }

    public override void Enter(Player player)
    {
        player.anim.SetBool("isMoving", true);
    }

    public override void Update(Player player)
    {
        player.StartCoroutine("LayerOpenClose");
        player.SetDash();
        player.SetParrying();
        player.SetWallsliding();

        if(!player.isDashing && !player.isParrying)
        {
            player.SetMoveState();
            player.Move();
            // if (player.rigid.velocity.y != 0)
            // {
            //     player.JumpAttack();
            // }
            // else
            // {
            //     player.Attack();
            // }
            player.JumpAttack();
            player.Attack();
            player.Jump();
            player.Stamp();
        }
    }

    public override void Exit(Player player)
    {
        player.anim.SetBool("isMoving", false);
    }
}

public class Jump : PlayerState
{
    public Jump() {}

    public Jump(PlayerStates ps)
    {
        ps = PlayerStates.JUMP;
        stateType = ps;
    }

    public override void Enter(Player player)
    {
        player.ParticleON(player.jumpAndLandDust);
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
        
        player.SetDash();
        player.SetWallsliding();

        if (!player.isDashing && !player.isParrying)
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
}

public class Attack : PlayerState
{
    public Attack() {}

    public Attack(PlayerStates ps)
    {
        ps = PlayerStates.ATTACK;
        stateType = ps;
    }

    public override void Enter(Player player)
    {
        player.anim.SetBool("canMove", false);
        player.rigid.velocity = new Vector2(0, 0);
        player.anim.SetTrigger("doAttack");
        player.anim.SetBool("isAttacking", true);
    }

    public override void Update(Player player)
    {
        player.SetDash();
        player.SetParrying();

        if (!player.isDashing && !player.isParrying) 
        {
            player.Attack();
            player.Slash();
        }
    }

    public override void Exit(Player player)
    {
        player.anim.SetBool("canMove", true);
        player.anim.SetBool("isAttacking", false);
    }
}

public class SlashAttack : PlayerState
{
    public SlashAttack() {}

    public SlashAttack(PlayerStates ps)
    {
        ps = PlayerStates.SLASHATTACK;
        stateType = ps;
    }

    public override void Enter(Player player)
    {
        player.ghost.makeGhost = true;
        player.anim.SetTrigger("doHeavyAttack");
    }

    public override void Update(Player player)
    {
    }

    public override void Exit(Player player)
    {
        if (!player.isRage)
        {
            player.ghost.makeGhost = false;
        }
    }
}

public class JumpAttack : PlayerState
{
    public JumpAttack() {}

    public JumpAttack(PlayerStates ps)
    {
        ps = PlayerStates.JUMPATTACK;
        stateType = ps;
    }

    public override void Enter(Player player)
    {
        player.anim.SetTrigger("doJumpAttack");
        player.rigid.velocity = new Vector2(0, 0);
    }

    public override void Update(Player player)
    {
        player.SetDash();
        player.SetWallsliding();

        if (!player.isDashing && !player.isParrying)
        {
            player.JumpAttack();
        }
        
        player.Stamp();
    }

    public override void Exit(Player player)
    {
    }
}

public class Stamping : PlayerState
{
    public Stamping() {}
    
    public Stamping(PlayerStates ps)
    {
        ps = PlayerStates.STAMPING;
        stateType = ps;
    }

    public override void Enter(Player player)
    {
        player.rigid.velocity = new Vector2(0,  -player.stampingPower);
    }

    public override void Update(Player player)
    {
    }

    public override void Exit(Player player)
    {
        player.StampAttack();
    }
}

public class Dash : PlayerState
{
    public Dash() {}

    public Dash(PlayerStates ps)
    {
        ps = PlayerStates.DASH;
        stateType = ps;
    }

    public override void Enter(Player player)
    {
        player.anim.SetBool("isDashing", true);
        player.isDashing = true;
        player.StartCoroutine("Dash");
        player.ghost.makeGhost = true;
    }

    public override void Update(Player player) {}

    public override void Exit(Player player)
    {
        if (!player.isRage)
        {
            player.ghost.makeGhost = false;
        }
        player.anim.SetBool("isDashing", false);
        player.anim.SetBool("canMove", true);
        player.isDashing = false;
        player.rigid.velocity = Vector2.zero;
    }
}

public class Parry : PlayerState
{
    public Parry() {}

    public Parry(PlayerStates ps)
    {
        ps = PlayerStates.PARRYING;
        stateType = ps;
    }

    public override void Enter(Player player)
    {
        player.rigid.velocity = new Vector2(0, 0);
        player.StartCoroutine("ParryingStart");
    }

    public override void Update(Player player) {}
    
    public override void Exit(Player player) {}
}

public class Wallsliding : PlayerState
{
    public Wallsliding() {}

    public Wallsliding(PlayerStates ps)
    {
        ps = PlayerStates.WALLSLIDING;
        stateType = ps;
    }

    public override void Enter(Player player)
    {
        player.isWallSliding = true;
        player.anim.SetBool("wallSliding", true);
    }

    public override void Update(Player player)
    {
        player.ParticleON(player.slideDust);
        player.rigid.velocity = new Vector2(player.rigid.velocity.x, player.rigid.velocity.y * player.slideSpeed);
        
        player.SetWalljumping();
        player.ExitWallSliding();
    }

    public override void Exit(Player player)
    {
        player.isWallSliding = false;
        player.anim.SetBool("wallSliding", false);
    }
}

public class Walljumping : PlayerState
{
    public Walljumping() {}

    public Walljumping(PlayerStates ps)
    {
        ps = PlayerStates.WALLJUMPING;
        stateType = ps;
    }

    public override void Enter(Player player)
    {
        player.isWallJumping = true;
        player.anim.SetBool("isJumping", true);
        player.anim.SetBool("isFalling", false);
        player.rigid.velocity = new Vector2(-player.transform.localScale.x * player.wallJumpPower * 0.7f, player.wallJumpPower);
        player.transform.localScale = new Vector2(-Mathf.Sign(player.transform.localScale.x), 1);
        player.Invoke("MoveLock", 0.3f);
    }

    public override void Update(Player player)
    {
        if (!player.isWallJumping)
        {
            player.Move();
        }

        player.SetWallsliding();
        player.SetDash();
        player.JumpAttack();
    }

    public override void Exit(Player player)
    {
        player.anim.SetBool("isJumping", false);
        player.isWallJumping = false;
    }
}

public class Dead : PlayerState
{
    public Dead() {}

    public Dead(PlayerStates ps)
    {
        ps = PlayerStates.DEAD;
        stateType = ps;
    }

    public override void Enter(Player player)
    {
        player.controlAble = false;
        player.anim.SetTrigger("isDead");
    }
    
    public override void Update(Player player) {}
    
    public override void Exit(Player player) {}
}
