using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class DialogManager : MonoBehaviour
{
    [TextArea(2, 5)]
    public DialogLine[] lines;
    [Header("UI")]
    public GameObject dialogCanvas;
    public TMP_Text HeaderText;
    public TMP_Text dialogText;
    public Button nextButton;
    public Image portraitImage;
    public PlayerMovementInput playerMovementInput;

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

    public void StartDialog(DialogLine[] newLines)
    {
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
