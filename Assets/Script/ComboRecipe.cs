using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Combo Recipe")]
public class ComboRecipe : ScriptableObject
{
    [Header("Recipe")]
    [Tooltip("The ordered list of cards required for this combo.")]
    public List<CardData> ingredients;

    [Header("Result")]
    [Tooltip("The card produced by this combo.")]
    public CardData result;
}
