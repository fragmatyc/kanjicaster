using System.Collections.Generic;
using UnityEngine;

public class ComboManager : MonoBehaviour
{
    public static ComboManager Instance;

    [Header("Configuration")]
    public List<ComboRecipe> recipes;

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

    /// <summary>
    /// Processes a list of cards and applies any valid combos.
    /// Combos are order-dependent.
    /// </summary>
    /// <param name="inputCards">The list of cards played by the player.</param>
    /// <returns>A new list of cards with combos applied.</returns>
    public List<CardData> ProcessCombos(List<CardData> inputCards)
    {
        // Create a copy of the list to modify
        List<CardData> processedCards = new List<CardData>(inputCards);
        bool comboFound = true;

        // Keep checking for combos until no more can be formed
        // This allows for multi-stage combos if we ever want them (A+B=C, C+D=E)
        // For now, it just ensures we catch all independent combos in the sequence
        while (comboFound)
        {
            comboFound = false;
            foreach (var recipe in recipes)
            {
                if (recipe.ingredients == null || recipe.ingredients.Count == 0) continue;

                // Look for the recipe sequence in the processedCards
                int matchIndex = FindSubsequenceIndex(processedCards, recipe.ingredients);

                if (matchIndex != -1)
                {
                    // Remove ingredients
                    processedCards.RemoveRange(matchIndex, recipe.ingredients.Count);

                    // Insert result at the position where the combo started
                    processedCards.Insert(matchIndex, recipe.result);

                    comboFound = true;
                    // Restart the search since the list has changed
                    // This prioritizes the first found combo in the list of recipes
                    // If we want specific priority, we should order the 'recipes' list accordingly
                    break;
                }
            }
        }

        return processedCards;
    }

    private int FindSubsequenceIndex(List<CardData> source, List<CardData> pattern)
    {
        if (pattern.Count > source.Count) return -1;

        for (int i = 0; i <= source.Count - pattern.Count; i++)
        {
            bool match = true;
            for (int j = 0; j < pattern.Count; j++)
            {
                if (source[i + j] != pattern[j])
                {
                    match = false;
                    break;
                }
            }
            if (match) return i;
        }

        return -1;
    }
}
