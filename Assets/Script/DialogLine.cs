using UnityEngine;
using TMPro;
using UnityEngine.UI;
[System.Serializable]
public class DialogLine
{
    public string speakerName;
    [TextArea(2, 4)]
    public string text;
    public Sprite portrait;
}
