using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using System.IO;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float jumpForce = 5f;
    public float gravity = -9.8f;
    public float coyoteTime = 0.2f; // Grace period to jump after leaving a platform
    public int doubleJumpAmount = 1; // Amount of double jumps per level


    [Header("Camera")]
    public float cameraDistance = 10f;
    public float cameraHeight = 5f;
    
    

    // Private variables
    private Transform cameraTransform;
    private CharacterController characterController;
    private Vector3 movement; // This will store our movement direction
    private Vector3 velocity;
    private float lastGroundedTime; // Track when the player was last grounded
    private bool isFallingToDeath = false;
    private bool cameraLockedOnDeath = false;
    private Vector3 deathCameraPosition;
    

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
        //doubleJumpAmount = 1;
        GameplayUI.Instance.UpdateDoubleJumpUI(doubleJumpAmount);
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

        // While falling to death, keep the camera locked in place
        // but still rotate it to look at the player.
        if (isFallingToDeath && cameraLockedOnDeath)
        {
            cameraTransform.position = deathCameraPosition;

            Vector3 toPlayer = transform.position - cameraTransform.position;
            if (toPlayer.sqrMagnitude > 0.0001f)
            {
                cameraTransform.rotation = Quaternion.LookRotation(toPlayer.normalized, Vector3.up);
            }
        }

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();
        
        float horizontalInput = 0f;
        float verticalInput = 0f;
        
        // Get keyboard input (disabled while falling into the void)
        if (!isFallingToDeath && Keyboard.current != null)
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
        bool spacePressed = !isFallingToDeath && keyboardNotNull && Keyboard.current.spaceKey.wasPressedThisFrame;
        bool spaceIsPressed = !isFallingToDeath && keyboardNotNull && Keyboard.current.spaceKey.isPressed;
        float velocityYBefore = velocity.y;
        Vector3 positionBeforeMove = transform.position;
        
        // Update lastGroundedTime when grounded
        if (wasGrounded) {
            lastGroundedTime = Time.time;
            
            velocity.y = 0;

            if (spacePressed) {
                velocity.y = jumpForce;
            }
        } else {
            // Check if we can still jump using coyote time
            bool canCoyoteJump = (Time.time - lastGroundedTime) < coyoteTime;
            
            if (spacePressed && (canCoyoteJump || doubleJumpAmount > 0)) {
                velocity.y = jumpForce;
                if (!canCoyoteJump) {
                    doubleJumpAmount--;
                    GameplayUI.Instance.UpdateDoubleJumpUI(doubleJumpAmount);
                }
            }
            
            velocity.y += gravity * Time.deltaTime;
        }
        
        // Combine horizontal movement with vertical velocity
        movement = horizontalMovement;
        movement.y = velocity.y * Time.deltaTime;

        characterController.Move(movement);

        // Maximum distance you can fall before triggering the death fall sequence
        if (!isFallingToDeath && GameManager.Instance != null && GameManager.Instance.levelDesigner != null)
        {
            float killZoneY = -GameManager.Instance.levelDesigner.heightVariation;
            if (transform.position.y < killZoneY)
            {
                isFallingToDeath = true;
                LockCameraPositionOnDeath();
                // Let the player continue falling for a short time,
                // then restart the level.
                StartCoroutine(DeathFallRoutine(1.2f));
            }
        }
        
    }

    private IEnumerator DeathFallRoutine(float delay) 
    {
        float elapsed = 0f;
        while (elapsed < delay)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        UnlockCameraAfterDeath();
        isFallingToDeath = false;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }

    private void LockCameraPositionOnDeath()
    {
        if (cameraLockedOnDeath)
        {
            return;
        }

        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            return;
        }

        var brain = mainCam.GetComponent<CinemachineBrain>();
        if (brain != null)
        {
            brain.enabled = false;
        }

        deathCameraPosition = mainCam.transform.position;
        cameraLockedOnDeath = true;
    }

    private void UnlockCameraAfterDeath()
    {
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            var brain = mainCam.GetComponent<CinemachineBrain>();
            if (brain != null)
            {
                brain.enabled = true;
            }
        }

        cameraLockedOnDeath = false;
    }
}
