using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingZone : MonoBehaviour
{
    public Transform respawnPoint;
    public Player player;
    public BoxCollider2D col;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player.TakeHit(1, this.transform);
            player.transform.position = respawnPoint.position;
        }
    }
}
