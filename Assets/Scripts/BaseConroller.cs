using Fusion;
using UnityEngine;

public class BaseController : NetworkBehaviour
{
    protected float health;
    protected float maxHealth;

    protected virtual void Start()
    {
        health = maxHealth;
    }

    protected virtual void Move() { }

    protected virtual void Attack() { }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Dead();
        }
    }

    protected virtual void Dead()
    {
        // Handle death
    }
}
