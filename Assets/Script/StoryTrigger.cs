using UnityEngine;

public class StoryTrigger : MonoBehaviour
{
    public DialogManager dialogManager;
    public string defaultResourcePathDialog;

    
    public ConditionalStory[] conditionalStories;
    
    bool hasTrigger = false;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (hasTrigger) return;
        if (!collider.CompareTag("Player")) return;

        string ressourcePathDialog = GetLinesForCurrentState();

        if (!string.IsNullOrEmpty(ressourcePathDialog) && dialogManager != null) {
             dialogManager.StartDialog(ressourcePathDialog);
             hasTrigger = true;
        }      

    }
    private string GetLinesForCurrentState()
    {
        var gs = GameState.Instance;
        if (gs == null)
        {
            Debug.LogWarning("GameState.Instance est null, on utilise le dialogue par défaut.");
            return defaultResourcePathDialog;
        }

        int deaths = gs.deathCount;
        int level = gs.playerLevel;

        foreach (var cond in conditionalStories)
        {
            if (deaths >= cond.minDeathCount &&
                deaths <= cond.maxDeathCount &&
                level >= cond.minLevel)               
            {
                return cond.ressourcePath;
            }
        }

        return defaultResourcePathDialog;
    }
}
