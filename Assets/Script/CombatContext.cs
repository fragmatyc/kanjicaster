using System;
using System.Collections.Generic;
using UnityEngine;

public enum TurnState
{
    Player,
    Enemy
}

public class CombatContext : MonoBehaviour
{
    public static CombatContext Instance;

    [Header("Who/what we fight")]
    public EnemyData enemyData;
    public List<CardData> enemyDeck;
    public List<CardData> enemyHand;
    public Player playerData;
    public List<CardData> playerHand;

    [Header("Return to exploration")]
    public string returnSceneName;
    public Vector3 playerReturnPosition;
    public bool enemyDefeated;
    public string enemyId;

    private TurnState turnState;

    private readonly List<CardData> currentCombo = new();

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

    void Start()
    {
        enemyDeck = enemyData.deck;
    }

    public void Clear()
    {
        enemyData = null;
        playerData = null;
        returnSceneName = "";
        enemyDefeated = false;
        enemyId = "";
        playerReturnPosition = Vector3.zero;
        currentCombo.Clear();
    }

    public void AddCardToCombo(CardData cardData)
    {
        currentCombo.Add(cardData);
    }

    public List<CardData> GetCombo()
    {
        return currentCombo;
    }

    public TurnState GetTurnState()
    {
        return turnState;
    }

    public void SetTurnState(TurnState state)
    {
        turnState = state;
    }

    public void NextTurn()
    {
        turnState = turnState == TurnState.Player ? TurnState.Enemy : TurnState.Player;
        currentCombo.Clear();
    }

    public CardData DrawCard()
    {
        CardData card;
        if (turnState == TurnState.Player)
        {
            card = playerData.deck[UnityEngine.Random.Range(0, playerData.deck.Count)];
        }
        else
        {
            card = enemyDeck[UnityEngine.Random.Range(0, enemyDeck.Count)];
        }

        return card;
    }
}
