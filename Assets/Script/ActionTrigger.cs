using UnityEngine;
using UnityEngine.InputSystem;

public class ActionTrigger : MonoBehaviour
{
    public string dialogueResourcePath;
    public DialogManager dialogManager;
    public string ObjectName;
    public string expectedActionParam = "GetMainCard";
    public string objectNeeded = "";
    public GameObject objectToDestroy= null;

    private bool playerInRange = false;
    
    bool alreadyTriggered = false;

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

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
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
            TriggerDialogue();
        }
    }
    private void TriggerDialogue()
    {
        if (dialogManager != null && !string.IsNullOrEmpty(dialogueResourcePath))
        {
            dialogManager.StartDialog(dialogueResourcePath);
        }
    }

    private void HandleChoiceExecuted(DialogueChoice choice)
    {
        if (choice == null) return;

        if (choice.actionType != ChoiceActionType.SetGameStateVariable){
            alreadyTriggered = false;
            return;
        }

        if (!string.IsNullOrEmpty(expectedActionParam) &&
            choice.actionParam != expectedActionParam)
        {
            return;
        }

        if (GameState.Instance != null) {
            switch (choice.actionType)
            {
                case ChoiceActionType.RefuseChoice:
                    // Do nothing
                    return;
                case ChoiceActionType.SetGameStateVariable:
                    if (choice.actionParam == "Ink") {
                        GameState.Instance.inkCapacity += 5;
                        return;
                    }                    
                    if (choice.actionParam == "MainCard") {
                        GameState.Instance.MainCard = ObjectName;
                        return;
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
                        dialogManager.StartDialog("Dialogs/door_locked");
                    }
                    return;
            }
        }
      
    }
}
