using UnityEngine;
using System.Collections.Generic;

public class GameState : MonoBehaviour
{
    public static GameState Instance;

    public int deathCount = 0;
    public int playerLevel = 1;
    public string MainCard = "";
    public int inkCapacity =10;
    public List<string> inventory = new List<string>();


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
