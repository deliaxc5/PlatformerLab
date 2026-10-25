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

    [SerializeField] private float coyoteTime = 0.1f;
    private float coyoteTimer;

    private InputAction jumpAction;
    private bool jumpRequested;
    private bool jumpCutRequested;
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int GroundedHash = Animator.StringToHash("IsGrounded");

    private Animator animator;

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
        animator = GetComponent<Animator>();
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
        if (jumpAction.WasPressedThisFrame())
        {
            jumpRequested = true;
        }

        if (jumpAction.WasReleasedThisFrame())
        {
            jumpCutRequested = true;
        }

        animator.SetFloat(SpeedHash, Mathf.Abs(rb.linearVelocity.x));
        animator.SetBool(GroundedHash, isGrounded);
    }

    void FixedUpdate()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position, groundCheckRadius, groundLayer) != null;
        if (isGrounded) coyoteTimer = coyoteTime;
        else coyoteTimer -= Time.fixedDeltaTime;
        if (jumpRequested && coyoteTimer > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);
            coyoteTimer = 0f;   // a granted jump spends the whole window
        }
        if (isGrounded != wasGrounded) Debug.Log($"isGrounded -> {isGrounded}");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (jumpRequested && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);
        }

        if (jumpCutRequested && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }

        jumpRequested = false;
        jumpCutRequested = false;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
