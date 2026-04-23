using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody2D rb;
    public PlayerInput playerInput;

    [Header("Movement Variables")]
    public float speed;
    public float jumpForce;
    public float jumpCutMultiplier; // Pour le saut variable (ex: 0.5f)
    
    [Header("Gravity Settings")]
    public float normalGravity;
    public float fallGravity;
    public float jumpGravity;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundLayer;
    public bool isGrounded;

    [Header("Input Tracking")]
    private Vector2 moveInput;
    private bool jumpPressed;
    private bool jumpReleased;
    private int facingDirection = 1;

    private void Start()
    {
        // Initialise la gravité au début du jeu
        rb.gravityScale = normalGravity;
    }

    private void Update()
    {
        Flip();
    }

    private void FixedUpdate()
    {
        // On gère la physique dans FixedUpdate
        CheckGrounded();
        HandleMovement();
        HandleJump();
        ApplyVariableGravity();
    }

    // --- LOGIQUE DE MOUVEMENT ---

    private void HandleMovement()
    {
        float targetSpeed = moveInput.x * speed;
        rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);
    }

    private void HandleJump()
    {
        // Saut initial
        if (jumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpPressed = false; // On consomme l'input
            jumpReleased = false;
        }

        // Coupure du saut (si on relâche le bouton en montant)
        if (jumpReleased)
        {
            if (rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
            }
            jumpReleased = false;
        }
    }

    private void ApplyVariableGravity()
    {
        if (rb.linearVelocity.y < -0.1f) // En train de tomber
        {
            rb.gravityScale = fallGravity;
        }
        else if (rb.linearVelocity.y > 0.1f) // En train de monter
        {
            rb.gravityScale = jumpGravity;
        }
        else // Au repos ou sur le sol
        {
            rb.gravityScale = normalGravity;
        }
    }

    // --- INPUTS (New Input System) ---

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            jumpPressed = true;
            jumpReleased = false;
        }
        else
        {
            jumpReleased = true;
            jumpPressed = false;
        }
    }

    // --- UTILITAIRES ---

    private void CheckGrounded()
    {
        // Crée un cercle invisible pour détecter le sol
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void Flip()
    {
        if (moveInput.x > 0.1f) facingDirection = 1;
        else if (moveInput.x < -0.1f) facingDirection = -1;

        transform.localScale = new Vector3(facingDirection, 1, 1);
    }

    // Permet de visualiser le Ground Check dans l'éditeur Unity
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}