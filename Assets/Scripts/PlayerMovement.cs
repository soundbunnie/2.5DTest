using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Fields
    // References
    [Header("References")]
    public SpriteRenderer spriteRenderer;
    PlayerInput playerInput;
    CharacterController characterController;

    // Movement
    bool isRunPressed;
    bool isMovementPressed;
    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    Vector3 appliedMovement;

    // Jump
    bool isJumpPressed = false;
    bool isJumping = false;
    bool fastFallPressed = false;
    float initialJumpVelocity;

    // Constants
    [Header("Movement")]
    public bool canMoveVertically = false;
    public float runSpeed = 3.0f;
    [Header("Jump")]
    public float maxJumpHeight = 1.0f;
    public float maxJumpTime = 0.5f;
    [Header("Gravity")]
    public float gravity = -9.8f;
    public float groundedGravity = -0.05f;
    #endregion

    private void Awake()
    {
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();

        playerInput.Player.Move.started += onMovementInput;
        playerInput.Player.Move.canceled += onMovementInput;
        playerInput.Player.Move.performed += onMovementInput;

        playerInput.Player.Run.started += onRun;
        playerInput.Player.Run.canceled += onRun;

        playerInput.Player.Jump.started += onJump;
        playerInput.Player.Jump.canceled += onJump;

        setupJumpVariables();
    }

    private void Update()
    {
        if (isRunPressed)
        {
            appliedMovement.x = currentRunMovement.x;
            appliedMovement.z = currentRunMovement.z;
        }
        else
        {
            appliedMovement.x = currentMovement.x;
            appliedMovement.z = currentMovement.z;
        }

        characterController.Move(appliedMovement * Time.deltaTime);
        handleGravity();
        handleJump();
    }

    private void FixedUpdate()
    {
        if (appliedMovement.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (appliedMovement.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    private void setupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    private void handleJump()
    {
        if (!isJumping && characterController.isGrounded && isJumpPressed)
        {
            isJumping = true;
            currentMovement.y = initialJumpVelocity;
            appliedMovement.y = initialJumpVelocity;
        }
        else if (!isJumpPressed && isJumping && characterController.isGrounded)
        {
            isJumping = false;
        }
    }

    private void handleGravity()
    {
        bool isFalling = currentMovement.y <= 0.0f || !isJumpPressed;
        float fallMultiplier = 4.0f;
        if (characterController.isGrounded)
        {
            currentMovement.y = groundedGravity;
            appliedMovement.y = groundedGravity;
        }
        else if (currentMovementInput.y < 0 && !characterController.isGrounded) // Fast fall
        {
            Debug.Log("Fast fall");
            float previousYVelocity = currentMovement.y;
            currentMovement.y = currentMovement.y + (gravity * fallMultiplier * Time.deltaTime);
            appliedMovement.y = Mathf.Max((previousYVelocity + currentMovement.y) * 0.5f, -20.0f);
        }
        else
        {
            float previousYVelocity = currentMovement.y;
            currentMovement.y = currentMovement.y + (gravity * Time.deltaTime);
            appliedMovement.y = (previousYVelocity + currentMovement.y) * 0.5f;
        }
    }

    private void onMovementInput (InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentRunMovement.x = currentMovement.x * runSpeed;

        if (canMoveVertically)
        {
            currentMovement.z = currentMovementInput.y;
            currentRunMovement.z = currentMovement.y * runSpeed;
        }
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    private void onJump (InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
    }

    private void onRun (InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }

    private void OnEnable()
    {
        playerInput.Player.Enable();
    }

    private void OnDisable()
    {
        playerInput.Player.Disable();
    }
}
