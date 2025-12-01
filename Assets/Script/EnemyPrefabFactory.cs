using UnityEngine;
public enum Enemies
{
    Undefined,
    FireSlime,
    FlyingDemon,
    Chibbi,
    Eyeball,
    Ghost,
    CaveBat,
    ManEaterPlant,
    Pumpking,
    Snake,
    Worm,
    Bee
}

public class EnemyPrefabFactory : MonoBehaviour
{
    [Header("Position")]
    public Transform enemyTransform;

    [Header("Monster Prefabs")]
    public GameObject chibbiPrefab;
    public GameObject fireSlimePrefab;
    public GameObject eyeballPrefab;
    public GameObject ghostPrefab;
    public GameObject caveBatPrefab;
    public GameObject manEaterPlantPrefab;
    public GameObject pumpkingPrefab;
    public GameObject snakePrefab;
    public GameObject wormPrefab;
    public GameObject beePrefab;

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
            Enemies.Chibbi => Instantiate(chibbiPrefab, enemyTransform.position, Quaternion.identity),
            Enemies.Eyeball => Instantiate(eyeballPrefab, enemyTransform.position, Quaternion.identity),
            Enemies.Ghost => Instantiate(ghostPrefab, enemyTransform.position, Quaternion.identity),
            Enemies.CaveBat => Instantiate(caveBatPrefab, enemyTransform.position, Quaternion.identity),
            Enemies.ManEaterPlant => Instantiate(manEaterPlantPrefab, enemyTransform.position, Quaternion.identity),
            Enemies.Pumpking => Instantiate(pumpkingPrefab, enemyTransform.position, Quaternion.identity),
            Enemies.Snake => Instantiate(snakePrefab, enemyTransform.position, Quaternion.identity),
            Enemies.Worm => Instantiate(wormPrefab, enemyTransform.position, Quaternion.identity),
            Enemies.Bee => Instantiate(beePrefab, enemyTransform.position, Quaternion.identity),
            Enemies.Undefined => null,
            _ => null,
        };
    }
}
