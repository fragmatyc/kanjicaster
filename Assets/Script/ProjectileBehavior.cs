using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    private int damage;
    private GameObject target;
    private CombatManager combatManager;
    private bool hasHit = false;

    public void Initialize(int damage, GameObject target, CombatManager manager)
    {
        this.damage = damage;
        this.target = target;
        this.combatManager = manager;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;
        if (collision.gameObject == target)
        {
            Hit();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasHit) return;
        if (collision.gameObject == target)
        {
            Hit();
        }
    }

    private void Hit()
    {
        hasHit = true;
        if (combatManager != null)
        {
            combatManager.OnProjectileHit(damage);
        }
        Destroy(gameObject);
    }
}
