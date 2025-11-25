using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI kanjiText;
    public TextMeshProUGUI descriptionText;
    public SpriteRenderer cardSpriteRenderer;
    public SpriteRenderer raritySpriteRenderer;

    public HandManager handManager;
    public int cardIndex;
    public CardData cardData;

    private void Awake()
    {
        handManager = GetComponentInParent<HandManager>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (handManager != null)
        {
            handManager.OnHover(cardIndex);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (handManager != null)
        {
            handManager.OnUnhover();
        }
    }

    void Start()
    {
        DisplayCard();
    }

    void DisplayCard()
    {
        if (cardNameText != null)
        {
            cardNameText.text = cardData.cardName;
        }
        if (kanjiText != null)
        {
            kanjiText.text = cardData.kanji;
        }
        if (descriptionText != null)
        {
            descriptionText.text = cardData.description;
        }

        switch (cardData.element)
        {
            case CardElement.Fire:
                cardSpriteRenderer.sprite = handManager.fireTypeCardSprite;
                break;
            case CardElement.Water:
                cardSpriteRenderer.sprite = handManager.waterTypeCardSprite;
                break;
            case CardElement.Wood:
                cardSpriteRenderer.sprite = handManager.woodTypeCardSprite;
                break;
            case CardElement.Normal:
                cardSpriteRenderer.sprite = handManager.normalTypeCardSprite;
                break;
        }

        switch (cardData.rarity)
        {
            case CardRarity.Common:
                raritySpriteRenderer.sprite = handManager.commonCardSprite;
                break;
            case CardRarity.Rare:
                raritySpriteRenderer.sprite = handManager.rareCardSprite;
                break;
            case CardRarity.Epic:
                raritySpriteRenderer.sprite = handManager.epicCardSprite;
                break;
        }

    }
}
