using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance;

    public int deathCount = 0;
    public int playerLevel = 1;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
