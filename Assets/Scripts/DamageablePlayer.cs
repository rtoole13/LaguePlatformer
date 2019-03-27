using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerState))]
public class DamageablePlayer : Damageable
{
    PlayerState playerState;
    void Start()
    {
        currentHealth = maxHealth;
        playerState = GetComponent<PlayerState>();
    }
    public override void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 1)
        {
            playerState.Kill();
        }
    }
}
