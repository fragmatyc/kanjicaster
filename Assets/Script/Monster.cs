using UnityEngine;
using UnityEngine.SceneManagement;

public class Monster : MonoBehaviour
{
    public string combatSceneName = "CombatScene";
    public float visionRange = 5f;
    public float combatTriggerRanger = 2f;
    public float moveSpeed = 2f;
    public LayerMask obstacleMask;
    public string enemyId;
    public EnemyData enemyData;
    public SceneTransitionManager sceneTransitionManager;

    private Transform target;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    void Awake()
    {
        var ctx = CombatContext.Instance;
        Debug.Log($"Loading Monster with enemyId: {enemyId}");
        Debug.Log($"ctx.enemyDefeated: {ctx.enemyDefeated}");
        Debug.Log($"ctx.enemyId: {ctx.enemyId}");
        if (ctx.enemyDefeated && ctx.enemyId == enemyId)
        {
            Destroy(gameObject);
        }

        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) target = p.transform;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, combatTriggerRanger);
    }

    void FixedUpdate()
    {
        if (target == null) return;

        Vector2 toPlayer = (Vector2)target.position - rb.position;
        float dist = toPlayer.magnitude;

        if (dist <= visionRange)
        {

            if (dist < combatTriggerRanger)
            {
                CollideWithPlayer();
                return;
            }

            RaycastHit2D hit = Physics2D.Raycast(
                rb.position,
                toPlayer.normalized,
                dist,
                obstacleMask
            );

            bool hasLineOfSight = hit.collider == null;

            if (hasLineOfSight)
            {
                Vector2 dir = toPlayer.normalized;
                rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
                if (dir.x < -0.01f)
                    sr.flipX = false;
                else if (dir.x > 0.01f)
                    sr.flipX = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        CollideWithPlayer();
    }

    private void CollideWithPlayer()
    {
        var ctx = CombatContext.Instance;
        ctx.enemyData = enemyData;
        ctx.returnSceneName = SceneManager.GetActiveScene().name;
        ctx.playerReturnPosition = target.transform.position;
        ctx.enemyDefeated = false;
        ctx.enemyId = enemyId;

        sceneTransitionManager.FadeToScene(combatSceneName);
    }
}
