using UnityEngine;
using UnityEngine.InputSystem;

public class ChangeCardManager : MonoBehaviour {
    public DialogManager dialogManager;
    public string dialogueResourcePath = "dialogs/addCard";
    private CardData cardDataSend;
    public bool alreadyTriggered = false;

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
        if (CombatContext.Instance == null) return;
        if (CombatContext.Instance.playerDeck.Count >= 5) {
            DialogueChoice[] choices = new DialogueChoice[CombatContext.Instance.playerDeck.Count];
            for (int i = 0; i < CombatContext.Instance.playerDeck.Count; i++) {
                CardData existingCard = CombatContext.Instance.playerDeck[i];
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
                    Debug.Log("Change card : " + cardDataSend.cardName);
                    if (CombatContext.Instance == null) return; 
                    CardData cardToRemove = CombatContext.Instance.playerDeck.Find(card => card.cardName == choice.actionParam);  
                    if (cardToRemove != null) {
                        CombatContext.Instance.playerDeck.Remove(cardToRemove);
                    }
                    CombatContext.Instance.playerDeck.Add(cardDataSend);   
                      Debug.Log("combatContext Lenght : " + CombatContext.Instance.playerDeck.Count);
                    return;
            }
        }
      
    }
}