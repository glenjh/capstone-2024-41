using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GroundChecking : MonoBehaviour
{
    private Player _player;
    private BoxCollider2D feet;
    public Vector2 rayBoxSize = new Vector2(0, 0);
    
    void Start()
    {
        _player = transform.parent.GetComponent<Player>();
        feet = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        Vector2 raycastOrigin = feet.bounds.center;
        Vector2 raycastDirection = Vector2.down;
        float raycastDistance = 0.01f;
        LayerMask layerMask = LayerMask.GetMask("Ground", "HoverGround","Fragment");

        // Perform the BoxCast
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(
            raycastOrigin,
            rayBoxSize,
            0f,
            raycastDirection,
            raycastDistance,
            layerMask
        );

        ///////////////////////////////////////////// Draw the ray
        
        // Vector2 halfSize = rayBoxSize * 0.5f;
        // Vector2 topLeft = raycastOrigin + new Vector2(-halfSize.x, halfSize.y);
        // Vector2 topRight = raycastOrigin + new Vector2(halfSize.x, halfSize.y);
        // Vector2 bottomLeft = raycastOrigin + new Vector2(-halfSize.x, -halfSize.y);
        // Vector2 bottomRight = raycastOrigin + new Vector2(halfSize.x, -halfSize.y);
        //
        // Debug.DrawLine(topLeft, topRight, Color.blue);
        // Debug.DrawLine(topRight, bottomRight, Color.blue);
        // Debug.DrawLine(bottomRight, bottomLeft, Color.blue);
        // Debug.DrawLine(bottomLeft, topLeft, Color.blue);

        if (raycastHit2D.collider != null)
        {
            _player.isGround = true;
        }
        else
        {
            _player.isGround = false;
        }

        if (_player.isGround && _player.groundCheckAble)
        {
            _player.rigid.gravityScale = _player.normalGravity;
            _player.anim.SetBool("isFalling", false);
            _player.anim.SetBool("isJumping", false);
            _player.anim.SetBool("wallSliding", false);
        }
    }
}
