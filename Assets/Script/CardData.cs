using UnityEngine;

public enum SpellAnimationType
{
    Undefined,
    AoEOnTarget,
    AoEOnCaster,
    CasterToTargetProjectile,
    SkyToTargetProjectile,
}

public enum CardElement
{
    Undefined,
    Fire,
    Water,
    Wood,
    Normal
}
public enum CardRarity
{
    Undefined,
    Common,
    Rare,
    Epic
}

public enum CardType
{
    Undefined,
    Element,
    Spell,
    Combo,
    Item
}

[CreateAssetMenu(menuName = "RPG/Card")]
public class CardData : ScriptableObject
{
    [Header("Card Text")]
    public string cardName;
    public string kanji;
    public string description;

    [Header("Card Type")]
    public CardRarity rarity;
    public CardType type;
    public CardElement element;

    [Header("Card Stats")]
    public int inkCost;
    public int attack;
    public int defense;
    public int health;

    [Header("Animation")]
    public GameObject animationPrefab;
    public float animationDuration = 1f;
    public SpellAnimationType animationType;
}
