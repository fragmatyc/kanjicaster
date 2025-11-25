using UnityEngine;

public class StoryTrigger : MonoBehaviour
{
    public DialogManager dialogManager;

    public DialogLine[] storyLines;

    bool hasTrigger = false;

    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("Entrer dans le trigger");
        if (hasTrigger) return;
        if (!collider.CompareTag("Player")) return;

        hasTrigger = true;
        Debug.Log("A �t� trigger");
        dialogManager.StartDialog(storyLines);

    }
}
