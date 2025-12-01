using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    private CardElement cardElement;
    private int damage;
    private GameObject target;
    private CombatManager combatManager;
    private bool hasHit = false;

    public void Initialize(int damage, GameObject target, CombatManager manager, CardElement cardElement)
    {
        this.damage = damage;
        this.target = target;
        this.combatManager = manager;
        this.cardElement = cardElement;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;

        // Check if the collided object is the target or a child of the target
        if (collision.transform.root == target.transform.root)
        {
            Hit();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasHit) return;
        // Check if the collided object is the target or a child of the target
        if (collision.gameObject.transform.root == target.transform.root)
        {
            Hit();
        }
    }

    private void Hit()
    {
        hasHit = true;
        if (combatManager != null)
        {
            combatManager.OnProjectileHit(damage, cardElement);
        }
        Destroy(gameObject);
    }
}
