using UnityEngine;
using UnityEngine.InputSystem;

public class WalkerController : MonoBehaviour
{
    [Header("Movement")]
    [Header("Jump")]
    [SerializeField] private float jumpVelocity = 15f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    private InputAction jumpAction;
    private bool jumpRequested;
    private bool isGrounded;
    [SerializeField] private float moveSpeed = 6f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private InputAction moveAction;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        Debug.Log($"Awake: rb={rb != null}, sr={spriteRenderer != null}, " +
                  $"move={moveAction != null}");
    }

    private float moveInput;
   
    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>().x;
        if (moveInput != 0f)
        {
            spriteRenderer.flipX = moveInput > 0f;
        }
        Debug.Log($"moveInput = {moveInput}");
    }

    void FixedUpdate()
    {
        // Move the astronaut horizontally
        rb.linearVelocity = new Vector2(
            moveInput * moveSpeed,
            rb.linearVelocity.y
        );

        // Check whether the astronaut is touching the ground
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
