using UnityEngine;
using UnityEngine.InputSystem;

public class ActionTrigger : MonoBehaviour
{
    public string dialogueResourcePath;
    public ChangeCardManager changeCardManager;
    public DialogManager dialogManager;
    public string ObjectName;
    public string expectedActionParam = "GetMainCard";
    public string objectNeeded = "";
    public GameObject objectToDestroy= null;
    public CardData cardData;

    private bool playerInRange = false;
    
    bool alreadyTriggered = false;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Debug.Log("Player in range of " + ObjectName);
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    void Update()
    {
        if (alreadyTriggered) return;
        if (!playerInRange) return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            dialogManager.OnChoiceSelectedDelegate += HandleChoiceExecuted;
            TriggerDialogue(dialogueResourcePath);
            
        }
    }
    private void TriggerDialogue(string dialogPath)
    {
        if (dialogManager != null && !string.IsNullOrEmpty(dialogPath))
        {
            dialogManager.StartDialog(dialogPath);
        }
    }

    private void HandleChoiceExecuted(DialogueChoice choice)
    {
        if (choice == null) return;
        

        if (choice.actionType == ChoiceActionType.RefuseChoice){
            alreadyTriggered = false;
            return;
        }

        if (!string.IsNullOrEmpty(expectedActionParam) &&
            choice.actionParam != expectedActionParam)
        {
            return;
        }

        if (GameState.Instance != null) {
            Debug.Log("choice action type " + choice.actionType);
            switch (choice.actionType)
            {
                case ChoiceActionType.SetGameStateVariable:                    
                    if (choice.actionParam == "MainCard") {
                        GameState.Instance.MainCard = ObjectName;
                        return;
                    } if (choice.actionParam == "NextCard") {
                        Debug.Log("changeCard  " + changeCardManager != null + " cardData " + cardData != null);
                         if (changeCardManager != null && cardData != null) {
                            changeCardManager.ChangeCard(cardData);
                         }                   
                    }                    
                    return;
                case ChoiceActionType.SetInventoryItem:
                    if (!GameState.Instance.inventory.Contains(ObjectName)) {
                        GameState.Instance.inventory.Add(ObjectName);
                    }
                    return;
                case ChoiceActionType.OpenDoor:
                    if (GameState.Instance.inventory.Contains(objectNeeded)) {
                        GameState.Instance.inventory.Remove(objectNeeded);
                        // Add logic to open the door here
                        objectToDestroy.SetActive(false);
                    } else {
                        TriggerDialogue("dialogs/door_locked");
                    }
                    return;
            }
        }
        dialogManager.OnChoiceSelectedDelegate -= HandleChoiceExecuted;
    }
}
