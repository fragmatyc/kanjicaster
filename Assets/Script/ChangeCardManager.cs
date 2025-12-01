using UnityEngine;
using UnityEngine.InputSystem;

public class ChangeCardManager : MonoBehaviour {
    public DialogManager dialogManager;
    public string dialogueResourcePath = "dialogs/addCard";
    [SerializeField]
    public CardData cardDataSend;
    public bool alreadyTriggered = false;
    public Player player;

    private void OnEnable()
    {
        if (dialogManager != null)
        {
            dialogManager.OnChoiceSelectedDelegate += HandleChoiceExecuted;
        }
    }

    private void OnDisable()
    {
        if (dialogManager != null)
        {
            dialogManager.OnChoiceSelectedDelegate -= HandleChoiceExecuted;
        }
    }
    
    
    public void ChangeCard(CardData cardData) {
        cardDataSend = cardData;
        Debug.Log("cardDataSend : " + cardDataSend.cardName);
        if (CombatContext.Instance == null) return;
        if (player.deck.Count >= 5) {
            DialogueChoice[] choices = new DialogueChoice[player.deck.Count];
            for (int i = 0; i < player.deck.Count; i++) {
                CardData existingCard = player.deck[i];
                choices[i] = new DialogueChoice {
                    text = "Remplacer " + existingCard.cardName + " par " + cardData.cardName,
                    actionType = ChoiceActionType.ChangeCard,
                    actionParam = existingCard.cardName
                };
            }
            choices[choices.Length -1] = new DialogueChoice {
                text = "Ne pas ajouter la carte " + cardData.cardName,
                actionType = ChoiceActionType.RefuseChoice,
                actionParam = "RefuseChangeCard"
            };           
            if (dialogManager != null) {
                dialogManager.StartDialog("dialogs/changeCard", choices);
            }
        } else {
            if (dialogManager != null) {
                DialogueChoice[] choices = new DialogueChoice[2];
                choices[0] = new DialogueChoice {
                    text = "Prendre " + cardData.cardName,
                    actionType = ChoiceActionType.ChangeCard,
                    actionParam = ""
                };
                choices[1] = new DialogueChoice {
                    text = "Jeter " + cardData.cardName,
                    actionType = ChoiceActionType.RefuseChoice,
                    actionParam = "RefuseChangeCard"
                };
                dialogManager.StartDialog(dialogueResourcePath,choices);
            }
        }
        
    }

    private void HandleChoiceExecuted(DialogueChoice choice)
    {
        if (choice == null) return;

        if (choice.actionType != ChoiceActionType.ChangeCard){
            alreadyTriggered = false;
            return;
        }

        if (GameState.Instance != null) {
            switch (choice.actionType)
            {
                case ChoiceActionType.RefuseChoice:
                    return;
                case ChoiceActionType.ChangeCard:   

                    CardData cardToRemove = player.deck.Find(card => card?.cardName == choice.actionParam);  
                    if (cardToRemove != null) {
                        player.deck.Remove(cardToRemove);
                    }
                    player.deck.Add(cardDataSend);
                    return;
            }
        }
      
    }
}