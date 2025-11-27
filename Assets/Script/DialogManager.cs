using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class DialogManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialogCanvas;
    public TMP_Text HeaderText;
    public TMP_Text dialogText;
    public Button nextButton;
    public Image portraitImage;
    public PlayerMovementInput playerMovementInput;
    private DialogLine[] lines;

    public event Action<DialogueChoice> OnChoiceSelectedDelegate;

    public PortraitDatabase portraitDatabase;

    [Header("Choices UI")]
    public GameObject choiceContainer;
    public GameObject choiceButtonPrefab;

    public float typingSpeed = 0.03f;
    public bool isActive = false;
    public bool isTyping = false;

    private Coroutine typingCoroutine;

    int currentIndex=0;

    void Start()
    {
        dialogCanvas.SetActive(isActive);
        if (nextButton != null)
            nextButton.onClick.AddListener(NextLine);
    }

    public DialogLine[] LoadDialogFromJSON(string ressourcesPath)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(ressourcesPath);
        if (jsonFile == null)
        {
            Debug.LogError("Fichier de dialogue non trouvé: " + ressourcesPath);
            return null;
        }

        DialogFileDTO file = JsonUtility.FromJson<DialogFileDTO>(jsonFile.text);
        if (file == null || file.dialogLines == null)
        {
            Debug.LogError("Dialogue JSON mal formé: " + ressourcesPath);
            return null;
        }

        DialogLine[] dialogLineFound = new DialogLine[file.dialogLines.Length];

        for (int i = 0; i < file.dialogLines.Length; i++)
        {
           DialogLineDto line = file.dialogLines[i];

           Sprite portraitSprite = null;
           if (!string.IsNullOrEmpty(line.portraitName) && portraitDatabase != null)
           {
               portraitSprite = portraitDatabase.GetPortraitByName(line.portraitName);
           }

           DialogueChoice[] choices = new DialogueChoice[line.choices != null ? line.choices.Length : 0];

           if (line.choices != null && line.choices.Length > 0)
           {
               for (int c = 0; c < line.choices.Length; c++)
               {
                   DialogueChoiceDto jsonChoice = line.choices[c];

                   ChoiceActionType actionType;
                   if (!Enum.TryParse(jsonChoice.actionType, true, out actionType))
                   {
                       actionType = ChoiceActionType.DialogDefault;
                   }

                   choices[c] = new DialogueChoice
                   {
                       text = jsonChoice.text,
                       nextLineIndex = jsonChoice.nextLineIndex,
                       actionType = actionType,
                       actionParam = jsonChoice.actionParam
                   };
               }
           }

           dialogLineFound[i] = new DialogLine
           {
               speakerName = line.speakerName,
               text = line.text,
               portrait = portraitSprite,
               choices = choices
           };
        }
        return dialogLineFound;
    }

    public void StartDialog(string ressourcesPath)
    {
        DialogLine[] newLines = LoadDialogFromJSON(ressourcesPath);
        if (newLines != null)
            lines = newLines;
        currentIndex = 0;
        isActive = true;
        dialogCanvas.SetActive(isActive);
        if (playerMovementInput != null)
            playerMovementInput.enabled = false;
        ShowCurrentLine();
    }

    public void ShowCurrentLine() {
        DialogLine line = lines[currentIndex];

        if (HeaderText != null)
            HeaderText.text = string.IsNullOrEmpty(line.speakerName)  ? "???" : line.speakerName;
        
        if (portraitImage != null)
            if (line.portrait != null) {
                portraitImage.sprite = line.portrait;
                portraitImage.enabled = true;
            }               
            else
                portraitImage.sprite = null;
        
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLine(line.text));

        isTyping = false;
        if(line.choices == null || line.choices.Length == 0)
        {
            nextButton.gameObject.SetActive(true);
            choiceContainer.SetActive(false);
        }
        else
        {
            nextButton.gameObject.SetActive(false);
            choiceContainer.SetActive(true);
            ShowChoices(line.choices);
        }
    }

    void ShowChoices(DialogueChoice[] choices)
    {
        foreach(Transform child in choiceContainer.transform)
        {
            Destroy(child.gameObject);
        }

        foreach(var choice in choices)
        {
            GameObject choiceButtonObj = Instantiate(choiceButtonPrefab, choiceContainer.transform);
            TMP_Text choiceText = choiceButtonObj.GetComponentInChildren<TMP_Text>();
            if (choiceText != null) {
                choiceText.text = choice.text;
                choiceText.enabled = true;
            }
            Image image = choiceButtonObj.GetComponent<Image>();
            if (image != null)
                image.enabled = true;

            Button button = choiceButtonObj.GetComponent<Button>();
            if (button != null)
            {
                Debug.Log("Adding listener for choice: " + choice.text);
                button.onClick.AddListener(() => {
                    Debug.Log("Choice clicked: " + choice.text);
                    OnChoiceSelected(choice);
                });
                button.enabled = true;
            }
            choiceButtonObj.SetActive(true);
        }
    }


    void OnChoiceSelected(DialogueChoice choice)
    {
        if (isTyping)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            dialogText.text = lines[currentIndex].text;
            isTyping = false;
            return;
        }
        HandleChoiceAction(choice);
        if (choice.nextLineIndex < 0 )
        {
            EndDialog();
            return;
        }
        currentIndex = choice.nextLineIndex;
        ShowCurrentLine();
    }

    private void HandleChoiceAction(DialogueChoice choice)
    {
            OnChoiceSelectedDelegate?.Invoke(choice);
    }

    private System.Collections.IEnumerator TypeLine(string lineText)
    {
        isTyping = true;
        dialogText.text = "";
        foreach (char c in lineText.ToCharArray())
        {
            dialogText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    public void NextLine()
    {
        if (!isActive) return;

        if (isTyping)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);
            dialogText.text = lines[currentIndex].text;
            isTyping = false;
            return;
        }
        currentIndex++;
        if(currentIndex >= lines.Length)
        {
            EndDialog();
        }
        else
        {
            ShowCurrentLine();
        }
    }
    void EndDialog()
    {
        isActive = false;
        dialogCanvas.SetActive(isActive);
        playerMovementInput.enabled = true;
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
    }

}
