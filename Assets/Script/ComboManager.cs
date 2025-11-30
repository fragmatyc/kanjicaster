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

        // Sort recipes by ingredient count descending to ensure specific combos are checked before generic ones
        if (recipes != null)
        {
            recipes.Sort((a, b) =>
            {
                if (a.ingredients == null && b.ingredients == null) return 0;
                if (a.ingredients == null) return 1;
                if (b.ingredients == null) return -1;
                return b.ingredients.Count.CompareTo(a.ingredients.Count);
            });
        }

        Debug.Log("ComboManager initialized with " + recipes.Count + " recipes");
        foreach (var recipe in recipes)
        {
            Debug.Log("Recipe: " + recipe.result.cardName);
            foreach (var ingredient in recipe.ingredients)
            {
                Debug.Log("  Ingredient: " + ingredient.cardName);
            }
        }
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

    public CardData GetBestCardToPlay(List<CardData> hand)
    {
        if (hand == null || hand.Count == 0) return null;

        // Prioritize using MORE cards: Check subsets of size 3, then 2, then 1
        for (int size = hand.Count; size >= 1; size--)
        {
            var combinations = GetCombinations(hand, size);

            CardData bestInTier = null;
            int maxDamage = -1;

            foreach (var combo in combinations)
            {
                // Check all permutations of this combination
                var perms = GetPermutations(combo);
                foreach (var perm in perms)
                {
                    List<CardData> result = ProcessCombos(perm);

                    // User requirement: If result has more than 1 card, it's not a valid single combo -> Skip
                    if (result.Count > 1) continue;

                    // We found a valid result (single card output)
                    CardData resultCard = result[0];
                    if (resultCard == null) continue;

                    // Since we iterate sizes descending, any valid result here is in the highest possible Tier
                    // We just need to maximize damage within this Tier
                    if (resultCard.attack > maxDamage)
                    {
                        maxDamage = resultCard.attack;
                        bestInTier = resultCard;
                    }
                }
            }

            // If we found anything at this Tier, return the best one immediately
            // This satisfies "1. Best 3-card, 2. If none, best 2-card..."
            if (bestInTier != null)
            {
                return bestInTier;
            }
        }

        return null;
    }

    private List<List<CardData>> GetCombinations(List<CardData> list, int length)
    {
        List<List<CardData>> result = new();
        GetCombinationsRecursive(list, length, 0, new(), result);
        return result;
    }

    private void GetCombinationsRecursive(List<CardData> list, int length, int start, List<CardData> current, List<List<CardData>> result)
    {
        if (current.Count == length)
        {
            result.Add(new(current));
            return;
        }

        for (int i = start; i < list.Count; i++)
        {
            current.Add(list[i]);
            GetCombinationsRecursive(list, length, i + 1, current, result);
            current.RemoveAt(current.Count - 1);
        }
    }

    private List<List<CardData>> GetPermutations(List<CardData> list)
    {
        List<List<CardData>> results = new();
        if (list.Count == 0) return results;
        if (list.Count == 1)
        {
            results.Add(new(list));
            return results;
        }

        for (int i = 0; i < list.Count; i++)
        {
            CardData card = list[i];
            List<CardData> remaining = new(list);
            remaining.RemoveAt(i); // Remove by index to handle duplicates correctly

            List<List<CardData>> subPerms = GetPermutations(remaining);
            foreach (var sub in subPerms)
            {
                sub.Insert(0, card);
                results.Add(sub);
            }
        }
        return results;
    }
}
