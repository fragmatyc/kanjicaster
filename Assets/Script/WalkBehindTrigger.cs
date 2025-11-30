using UnityEngine;

public class WalkBehindTrigger : MonoBehaviour
{
    public DialogManager dialogManager;
    public string defaultResourcePathDialog;

    public SceneTransitionManager sceneTransitionManager;

    public string expectedActionParam="FireStoryLineLabyrinth";

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
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (alreadyTriggered) return;
        if (!other.CompareTag("Player")) return;

        alreadyTriggered = true;

        if (dialogManager != null && !string.IsNullOrEmpty(defaultResourcePathDialog))
        {
            dialogManager.StartDialog(defaultResourcePathDialog);
        }
    }
    private void HandleChoiceExecuted(DialogueChoice choice)
    {
        if (choice == null) return;

        if (choice.actionType != ChoiceActionType.ChangeScene){
            alreadyTriggered = false;
            return;
        }

        if (!string.IsNullOrEmpty(expectedActionParam) &&
            choice.actionParam != expectedActionParam)
        {
            return;
        }

        if (sceneTransitionManager != null)
        {
            sceneTransitionManager.FadeToScene(choice.actionParam);
        }
    }
}
