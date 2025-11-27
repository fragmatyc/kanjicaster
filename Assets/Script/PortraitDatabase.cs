using UnityEngine;

[CreateAssetMenu(menuName = "StoryTelling/Dialogs/PortraitDatabase")]
public class PortraitDatabase : ScriptableObject
{
    [System.Serializable]
    public class PortraitEntry
    {
        public string characterName;
        public Sprite portrait;
    }

    public PortraitEntry[] portraits;

    public Sprite GetPortraitByName(string name)
    {
        foreach (var entry in portraits)
        {
            if (entry.characterName == name)
            {
                Debug.LogWarning("Portrait found for character: " + name);
                return entry.portrait;
            }
            Debug.LogWarning("Checking portrait entry: " + entry.characterName);
        }
        Debug.LogWarning("Portrait not found for character: " + name);
        return null;
    }
}
