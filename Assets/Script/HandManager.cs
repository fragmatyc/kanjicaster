using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HandManager : MonoBehaviour
{
    public Sprite fireTypeCardSprite;
    public Sprite waterTypeCardSprite;
    public Sprite woodTypeCardSprite;
    public Sprite normalTypeCardSprite;

    public Sprite commonCardSprite;
    public Sprite rareCardSprite;
    public Sprite epicCardSprite;

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OnHover(int cardIdx)
    {
        Debug.Log($"Hovering over card {cardIdx}");
        animator.SetInteger("HoverIndex", cardIdx);
    }

    public void OnUnhover()
    {
        Debug.Log("Unhovering");
        animator.SetInteger("HoverIndex", 0);
    }
}
