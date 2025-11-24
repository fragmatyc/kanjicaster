using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [TextArea(2, 5)]
    public string[] lines;
    public GameObject dialogCanvas;
    public TMP_Text dialogText;
    public Button nextButton;

    int currentIndex=0;
    bool isActive = false;

    void Start()
    {
        dialogCanvas.SetActive(isActive);
        nextButton.onClick.AddListener(NextLine);
    }

    public void StartDialog(string[] newLines)
    {
        lines = newLines;
        currentIndex = 0;
        isActive = true;
        dialogCanvas.SetActive(isActive);
        dialogText.text = lines[currentIndex];
    }

    public void NextLine()
    {
        if (!isActive) return;

        currentIndex++;
        if(currentIndex >= lines.Length)
        {
            EndDialog();
        }
        else
        {
            dialogText.text = lines[currentIndex];
        }
    }
    void EndDialog()
    {
        isActive = false;
        dialogCanvas.SetActive(isActive);
    }
}
