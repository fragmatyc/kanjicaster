using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float transitionDuration = 1f;
    [SerializeField] private PlayerMovementInput playerMovementInput;
    private static readonly int FadeOutTrigger = Animator.StringToHash("FadeOut");

    public void FadeToScene(string sceneName)
    {
        if (playerMovementInput != null)
        {
            playerMovementInput.SetEnabled(false);
        }
        StartCoroutine(FadeAndLoad(sceneName));
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        animator.ResetTrigger(FadeOutTrigger);
        animator.SetTrigger(FadeOutTrigger);

        yield return new WaitForSeconds(transitionDuration);

        SceneManager.LoadScene(sceneName);
    }

}