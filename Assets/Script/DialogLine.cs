using UnityEngine;
using TMPro;
using UnityEngine.UI;


public enum ChoiceActionType
{
    DialogDefault,
    ChangeScene,
    SetGameStateVariable
}

[System.Serializable]
public class DialogueChoice
{
    public string text;  
    public int nextLineIndex = -1;

    public ChoiceActionType actionType = ChoiceActionType.DialogDefault;
    public string actionParam;
}


[System.Serializable]
public class DialogLine
{
    public string speakerName;
    [TextArea(2, 4)]
    public string text;
    public Sprite portrait;

    public DialogueChoice[] choices;
}


[System.Serializable]
public class DialogFileDTO {
    public DialogLineDto[] dialogLines;
}


[System.Serializable]
public class DialogLineDto {
    public string speakerName;
    [TextArea(2, 4)]
    public string text;
    public string portraitName;

    public DialogueChoiceDto[] choices;
}

[System.Serializable]
public class DialogueChoiceDto
{
    public string text;  
    public int nextLineIndex = -1;

    public string actionType;
    public string actionParam;
}