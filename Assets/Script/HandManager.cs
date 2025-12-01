using System;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HandManager : MonoBehaviour
{
    [Header("Cards in hand")]
    public Card firstCard;
    public Card secondCard;
    public Card thirdCard;

    [Header("UI")]
    public TextMeshProUGUI kanjiPlayedText;
    public Animator kanjiPlayedAnimator;

    [Header("Sprites")]
    public Sprite fireTypeCardSprite;
    public Sprite waterTypeCardSprite;
    public Sprite woodTypeCardSprite;
    public Sprite normalTypeCardSprite;

    public Sprite commonCardSprite;
    public Sprite rareCardSprite;
    public Sprite epicCardSprite;

    [Header("References")]
    public GameObject endTurnButton;
    public CombatManager combatManager;

    private Animator animator;
    private bool isKanjiAnimationPlaying = false;
    private bool isHidingCard = false;


    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Draw()
    {
        var ctx = CombatContext.Instance;
        List<CardData> hand;
        if (ctx.GetTurnState() == TurnState.Enemy)
        {
            hand = ctx.enemyHand;
        }
        else
        {
            hand = ctx.playerHand;
        }

        if (hand.Count >= 3)
        {
            return;
        }

        var card = ctx.DrawCard();
        hand.Add(card);
        RefreshCardSlots();

        if (hand.Count < 3)
        {
            Draw();
        }
    }

    public void OnHover(int cardIdx)
    {
        var animationIdx = animator.GetInteger("AnimationIndex");
        if (animationIdx != 0) return;
        Debug.Log($"Hovering over card {cardIdx}");
        animator.SetInteger("AnimationIndex", cardIdx);
    }

    public void OnUnhover()
    {
        var animationIdx = animator.GetInteger("AnimationIndex");
        if (!(animationIdx > 0 && animationIdx < 4)) return;
        Debug.Log($"Unhovering card {animationIdx}");
        animator.SetInteger("AnimationIndex", 0);
    }
    public void OnClick(int cardIdx)
    {
        if (CombatContext.Instance.GetTurnState() != TurnState.Player) return;
        Card cardPlayed = cardIdx switch
        {
            1 => firstCard,
            2 => secondCard,
            3 => thirdCard,
            _ => null
        };
        if (cardPlayed == null) return;

        List<CardData> combo = new();
        combo.AddRange(CombatContext.Instance.GetCombo());
        combo.Add(cardPlayed.cardData);

        var comboManager = ComboManager.Instance;
        var comboResult = comboManager.ProcessCombos(combo);
        if (comboResult.Count > 1)
        {

            Debug.Log("Invalid Combo");
            foreach (var result in comboResult)
            {
                Debug.Log(result.cardName);
            }
            return;
        }
        Debug.Log($"Clicking on card {cardIdx} and playing animation {cardIdx + 3}");
        animator.SetInteger("AnimationIndex", cardIdx + 3);
    }

    public void OnCardHitTable(int cardIdx)
    {
        // Prevent setting the trigger multiple times
        if (isKanjiAnimationPlaying) return;

        Debug.Log($"Card {cardIdx} hit the table");
        Card cardPlayed = cardIdx switch
        {
            1 => firstCard,
            2 => secondCard,
            3 => thirdCard,
            _ => null
        };
        if (cardPlayed == null) return;

        isKanjiAnimationPlaying = true;
        kanjiPlayedText.text = cardPlayed.cardData.kanji;

        cardPlayed.GetComponentInChildren<ParticleSystem>().Play();

        // Set color based on element (using custom hex colors)
        Color kanjiColor;
        switch (cardPlayed.cardData.element)
        {
            case CardElement.Fire:
                ColorUtility.TryParseHtmlString("#C4232F", out kanjiColor);
                break;
            case CardElement.Water:
                ColorUtility.TryParseHtmlString("#0069AA", out kanjiColor);
                break;
            case CardElement.Wood:
                ColorUtility.TryParseHtmlString("#32984A", out kanjiColor);
                break;
            case CardElement.Normal:
                ColorUtility.TryParseHtmlString("#adadad", out kanjiColor);
                break;
            default:
                kanjiColor = Color.white;
                break;
        }
        kanjiPlayedText.color = kanjiColor;

        // Play sound
        kanjiPlayedAnimator.SetTrigger("CardPlayed");
    }

    public void HideCard(int cardIdx)
    {
        // Prevent being called multiple times
        if (isHidingCard) return;
        isHidingCard = true;

        Debug.Log($"HideCard called with cardIdx={cardIdx}");
        Card cardToHide = cardIdx switch
        {
            1 => firstCard,
            2 => secondCard,
            3 => thirdCard,
            _ => null
        };

        Debug.Log($"cardToHide is {(cardToHide == null ? "null" : "not null")}, " +
                  $"cardData is {(cardToHide?.cardData == null ? "null" : cardToHide.cardData.kanji)}, " +
                  $"active is {cardToHide?.gameObject.activeSelf}");

        if (cardToHide == null || cardToHide.cardData == null || !cardToHide.gameObject.activeSelf)
        {
            Debug.LogWarning($"Cannot hide card {cardIdx} - validation failed");
            isHidingCard = false;
            return;
        }

        // Reset states
        isKanjiAnimationPlaying = false;
        kanjiPlayedAnimator.ResetTrigger("CardPlayed");
        kanjiPlayedAnimator.Play("KanjiCardPlayedIdle");

        // Hide the card and show button
        cardToHide.gameObject.SetActive(false);
        if (!endTurnButton.activeSelf)
        {
            endTurnButton.SetActive(true);
        }

        if (!combatManager.comboIndicator.activeSelf)
        {
            combatManager.comboIndicator.SetActive(true);
        }

        GameObject cardIndicator = null;
        if (!combatManager.firstCardComboIndicator.activeSelf)
        {
            cardIndicator = combatManager.firstCardComboIndicator;
        }
        else if (!combatManager.secondCardComboIndicator.activeSelf)
        {
            cardIndicator = combatManager.secondCardComboIndicator;
        }
        else if (!combatManager.thirdCardComboIndicator.activeSelf)
        {
            cardIndicator = combatManager.thirdCardComboIndicator;
        }

        if (cardIndicator != null)
        {
            cardIndicator.SetActive(true);
            cardIndicator.GetComponentInChildren<TextMeshProUGUI>().text = cardToHide.cardData.kanji;
        }


        // Force stop the hand animator by resetting to idle
        animator.SetInteger("AnimationIndex", 0);

        // Re-enable after a frame
        isHidingCard = false;

        CombatContext.Instance.AddCardToCombo(cardToHide.cardData);
        CombatContext.Instance.playerHand.Remove(cardToHide.cardData);
        RefreshCardSlots();
    }

    private void RefreshCardSlots()
    {
        var ctx = CombatContext.Instance;
        if (ctx.GetTurnState() == TurnState.Enemy)
        {
            return;
        }

        List<CardData> hand = ctx.playerHand;

        // Slot 1
        if (hand.Count > 0)
        {
            firstCard.cardData = hand[0];
            firstCard.gameObject.SetActive(true);
            firstCard.DisplayCard();
        }
        else
        {
            firstCard.cardData = null;
            firstCard.gameObject.SetActive(false);
        }

        // Slot 2
        if (hand.Count > 1)
        {
            secondCard.cardData = hand[1];
            secondCard.gameObject.SetActive(true);
            secondCard.DisplayCard();
        }
        else
        {
            secondCard.cardData = null;
            secondCard.gameObject.SetActive(false);
        }

        // Slot 3
        if (hand.Count > 2)
        {
            thirdCard.cardData = hand[2];
            thirdCard.gameObject.SetActive(true);
            thirdCard.DisplayCard();
        }
        else
        {
            thirdCard.cardData = null;
            thirdCard.gameObject.SetActive(false);
        }
    }

}
