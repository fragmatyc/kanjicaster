using UnityEngine;

public class CombatManager : MonoBehaviour
{
    private EnemyData enemy;

    void Start()
    {
        enemy = CombatContext.Instance.enemyData;
        if (enemy == null) return;

        Debug.Log($"Encountered {enemy.name}");
    }

}
