using UnityEngine;

public class StoryTrigger : MonoBehaviour
{
    public DialogManager dialogManager;

    [TextArea(2, 5)]
    public string[] storyLines;

    bool hasTrigger = false;

    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("Entrer dans le trigger");
        if (hasTrigger) return;
        if (!collider.CompareTag("Player")) return;

        hasTrigger = true;
        Debug.Log("A été trigger");
        dialogManager.StartDialog(storyLines);

    }
}
