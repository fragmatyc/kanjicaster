using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "RPG/Enemy")]
public class EnemyData : ScriptableObject
{
    public int level = 1;
    public string enemyName;
    public int maxHP = 10;
    public int currentHP = 10;
    public int attack = 2;
    public int defense = 0;
    public int initiative = 0;
    public Enemies enemyType;
    public CardElement mainElement;
    public GameObject prefab;
    public List<CardData> deck;
}
