using UnityEngine;

public class CombatContext : MonoBehaviour
{
    public static CombatContext Instance;

    [Header("Who/what we fight")]
    public EnemyData enemyData;
    public Player playerData;

    [Header("Return to exploration")]
    public string returnSceneName;
    public Vector3 playerReturnPosition;
    public bool enemyDefeated;

    public string enemyId;
    public MonsterType type;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Clear()
    {
        enemyData = null;
        playerData = null;
        returnSceneName = "";
        enemyDefeated = false;
        enemyId = "";
        type = MonsterType.Undefined;
        playerReturnPosition = Vector3.zero;
    }
}
