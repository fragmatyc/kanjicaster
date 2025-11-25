using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementInput : MonoBehaviour
{
    public static PlayerMovementInput Instance;

    public float moveSpeed = 5f;
    private bool enabledMovement = true;


    private Rigidbody2D rb;
    private Vector2 moveInput;
    private PlayerInputActions inputActions;
    private Animator animator;
    private SpriteRenderer sr;

    public void SetEnabled(bool v) {
        enabledMovement = v;
        if (!v)
        {
            moveInput = Vector2.zero;
            if (rb != null) rb.linearVelocity = Vector2.zero;

            if (animator != null)
            {
                animator.SetBool("isMoving", false);
                animator.speed = 0f;
            }
        }
        else
        {
            if (animator != null)
                animator.speed = 1f;
        }
    }

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        inputActions = new PlayerInputActions();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;
        inputActions.Player.Disable();
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        
        if (!enabledMovement) return;
        moveInput = ctx.ReadValue<Vector2>();
        bool isMoving = moveInput.sqrMagnitude > 0.001f;
        animator.SetBool("isMoving", isMoving);

        if (moveInput.x < -0.01f)
            sr.flipX = true; 
        else if (moveInput.x > 0.01f)
            sr.flipX = false;
    }

    private void FixedUpdate()
    {
        if (!enabledMovement) return;
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }
}
