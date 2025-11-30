using UnityEngine;
public enum Enemies
{
    Undefined,
    FireSlime,
    FlyingDemon
}

public class EnemyPrefabFactory : MonoBehaviour
{
    [Header("Position")]
    public Transform enemyTransform;

    [Header("Monster Prefabs")]
    public GameObject fireSlimePrefab;

    [Header("Boss Prefabs")]
    public GameObject flyingDemonPrefab;

    public static EnemyPrefabFactory instance;
    private void Awake()
    {
        instance = this;
    }

    public GameObject CreateEnemy(EnemyData enemyData)
    {
        return enemyData.enemyType switch
        {
            Enemies.FireSlime => Instantiate(fireSlimePrefab, enemyTransform.position, Quaternion.identity),
            Enemies.FlyingDemon => Instantiate(flyingDemonPrefab, enemyTransform.position, Quaternion.identity),
            Enemies.Undefined => null,
            _ => null,
        };
    }
}
