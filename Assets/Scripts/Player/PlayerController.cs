using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using System.IO;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float jumpForce = 5f;
    public float gravity = -9.8f;
    public float coyoteTime = 0.2f; // Grace period to jump after leaving a platform


    [Header("Camera")]
    public float cameraDistance = 10f;
    public float cameraHeight = 5f;
    
    
    // Private variables
    private Transform cameraTransform;
    private CharacterController characterController;
    private Vector3 movement; // This will store our movement direction
    private Vector3 velocity;
    private float lastGroundedTime; // Track when the player was last grounded
    
    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        
        // Find the main Unity camera
        try {
        cameraTransform = Camera.main.transform;
        }
        catch {
        Debug.LogError("PlayerController: No Camera tagged 'MainCamera' found in the scene!");
        }
    }
    
    void Update()
    {
        // Don't process input if game is not in Playing state
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
        {
            return;
        }

        if (cameraTransform == null)
        {
            Debug.LogError("PlayerController: No cameraTransform assigned!");
            return;
        }
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();
        
        float horizontalInput = 0f;
        float verticalInput = 0f;
        
        // Get keyboard input
        if (Keyboard.current != null)
        {
            // Check if keys are pressed (returns true/false)
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                horizontalInput = 1f;  // Move right
            else if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                horizontalInput = -1f; // Move left
            
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                verticalInput = 1f;    // Move forward
            else if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                verticalInput = -1f;   // Move backward
        }

        // Handles horizontal movement (X and Z axes)
        Vector3 horizontalMovement = horizontalInput * cameraRight + verticalInput * cameraForward;

        if (horizontalMovement.magnitude > 0.1f) {
            transform.rotation = Quaternion.LookRotation(horizontalMovement);
            horizontalMovement = horizontalMovement.normalized * speed * Time.deltaTime;
        } else {
            horizontalMovement = Vector3.zero;
        }
        
        // Handles gravity and jumping (Y-axis stuff)
        bool wasGrounded = characterController.isGrounded;
        bool keyboardNotNull = Keyboard.current != null;
        bool spacePressed = keyboardNotNull && Keyboard.current.spaceKey.wasPressedThisFrame;
        bool spaceIsPressed = keyboardNotNull && Keyboard.current.spaceKey.isPressed;
        float velocityYBefore = velocity.y;
        Vector3 positionBeforeMove = transform.position;
        
        // Update lastGroundedTime when grounded
        if (wasGrounded) {
            lastGroundedTime = Time.time;
            
            if (spacePressed) {
                velocity.y = jumpForce;
            }
        } else {
            // Check if we can still jump using coyote time
            bool canCoyoteJump = (Time.time - lastGroundedTime) < coyoteTime;
            
            if (spacePressed && canCoyoteJump) {
                velocity.y = jumpForce;
            }
            
            velocity.y += gravity * Time.deltaTime;
        }
        
        // Combine horizontal movement with vertical velocity
        movement = horizontalMovement;
        movement.y = velocity.y * Time.deltaTime;

        characterController.Move(movement);
        
    }
}
