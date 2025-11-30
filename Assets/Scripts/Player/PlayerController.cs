using UnityEngine;
using UnityEngine.InputSystem;

// ============================================================================
// CHARACTER CONTROLLER vs REGULAR COLLIDERS:
// ============================================================================
// 
// CharacterController is a SPECIALIZED component for player/character movement.
// Here's why we use it instead of a regular collider (BoxCollider, CapsuleCollider, etc.):
//
// REGULAR COLLIDERS (BoxCollider, SphereCollider, etc.):
// - Used with Rigidbody physics system
// - Objects are pushed by forces, collisions, and physics
// - Movement is handled by Unity's physics engine (which can feel floaty/slippery)
// - Good for: Objects that should bounce, fall naturally, or be affected by physics
// - Example: A crate that gets pushed around, a ball that rolls
//
// CHARACTER CONTROLLER:
// - NO Rigidbody needed - movement is DIRECT and immediate
// - You control movement manually with .Move() method
// - Has built-in collision detection (stops you from going through walls)
// - Has built-in .isGrounded property (tells you if touching ground)
// - Movement feels snappy and responsive (perfect for platformers)
// - Handles slopes and stairs automatically (won't get stuck on small bumps)
// - Good for: Player characters, NPCs, anything that needs precise control
//
// KEY DIFFERENCE:
// - Collider + Rigidbody = Physics controls movement (realistic but less control)
// - CharacterController = YOU control movement (precise and responsive)
//
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    
    // Private variables - components we need to reference
    private CharacterController characterController;
    private Vector3 movement; // This will store our movement direction
    private Vector3 velocity;
    public float jumpForce = 5f;
    public float gravity = -9.8f;
    
    // ============================================================================
    // Unity Lifecycle Methods (these are called automatically by Unity)
    // ============================================================================
    
    void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }
    
    void Update()
    {
        // ============================================================================
        // NEW INPUT SYSTEM - Simple approach using Keyboard.current
        // ============================================================================
        // The new Input System works differently than the old Input.GetAxis()
        // Instead, we check Keyboard.current which gives us the current keyboard state
        //
        // Keyboard.current returns null if no keyboard is connected, so check first!
        
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

        // Create horizontal movement (X and Z only)
        Vector3 horizontalMovement = new Vector3(horizontalInput, 0, verticalInput);

        if (horizontalMovement.magnitude > 0.1f) {
            transform.rotation = Quaternion.LookRotation(horizontalMovement);
            horizontalMovement = horizontalMovement.normalized * speed * Time.deltaTime;
        } else {
            horizontalMovement = Vector3.zero;
        }
        
        // Handle gravity and jumping (affects Y axis)
        if (characterController.isGrounded) {
            velocity.y = -0.2f; // Small downward force to keep grounded
            // Check jump with new Input System
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) {
                velocity.y = jumpForce;
            }
        } else {
            velocity.y += gravity * Time.deltaTime;
        }
        
        // Combine horizontal movement with vertical velocity
        movement = horizontalMovement;
        movement.y = velocity.y * Time.deltaTime;

        characterController.Move(movement);
    }
    
    // ============================================================================
    // INPUT.GetAxis() - EXPLAINED IN DETAIL:
    // ============================================================================
    //
    // Input.GetAxis("Horizontal") or Input.GetAxis("Vertical") returns a FLOAT value.
    // 
    // HOW IT WORKS:
    // - Returns a value between -1.0 and 1.0
    // - Returns 0.0 when no key is pressed
    // - The value SMOOTHLY transitions (not instant on/off)
    //   * Pressing D or Right Arrow: gradually goes from 0 → 1.0
    //   * Pressing A or Left Arrow: gradually goes from 0 → -1.0
    //   * Releasing: gradually returns to 0.0
    //
    // "Horizontal" axis:
    //   - Positive (0 to 1): D key OR Right Arrow
    //   - Negative (-1 to 0): A key OR Left Arrow
    //   - Zero (0): Nothing pressed
    //
    // "Vertical" axis:
    //   - Positive (0 to 1): W key OR Up Arrow
    //   - Negative (-1 to 0): S key OR Down Arrow
    //   - Zero (0): Nothing pressed
    //
    // EXAMPLE VALUES:
    //   Input.GetAxis("Horizontal") might return:
    //   - 0.0 (no input)
    //   - 0.5 (halfway pressed)
    //   - 1.0 (fully pressed right)
    //   - -0.8 (mostly pressed left)
    //
    // WHY USE THIS?
    // - Analog input support (gamepad sticks give precise 0.0-1.0 values)
    // - Smooth movement (gradual acceleration/deceleration feels natural)
    // - Can check both keys at once (if you somehow pressed A+D, would average out)
    //
    // ALTERNATIVE: Input.GetAxisRaw()
    // - Same thing BUT returns instantly -1, 0, or 1 (no smoothing)
    // - Use when you want snappy, instant response
    
    // ============================================================================
    // VECTOR3 - EXPLAINED IN DETAIL:
    // ============================================================================
    //
    // Vector3 represents a point or direction in 3D space with X, Y, Z coordinates.
    //
    // THINK OF IT LIKE:
    // - A point on a 3D graph: (x, y, z)
    // - OR a direction/arrow pointing somewhere
    //
    // THE THREE COMPONENTS:
    //   Vector3 movement = new Vector3(x, y, z);
    //   - x = Left/Right (red arrow in Unity)
    //   - y = Up/Down (green arrow in Unity) 
    //   - z = Forward/Backward (blue arrow in Unity)
    //
    // CREATING VECTORS:
    //   Vector3 pos1 = new Vector3(5, 10, 3);     // Point at (5, 10, 3)
    //   Vector3 pos2 = new Vector3(0, 0, 0);      // Origin point
    //   Vector3 pos3 = Vector3.zero;              // Same as (0, 0, 0) - shorthand!
    //   Vector3 dir1 = new Vector3(1, 0, 0);      // Direction pointing right
    //   Vector3 dir2 = Vector3.up;                // Shorthand for (0, 1, 0)
    //
    // ACCESSING COMPONENTS:
    //   Vector3 player = new Vector3(5, 2, 10);
    //   float x = player.x;  // Gets 5
    //   float y = player.y;  // Gets 2
    //   float z = player.z;  // Gets 10
    //
    // MODIFYING COMPONENTS:
    //   Vector3 move = new Vector3(1, 0, 0);
    //   move.y = 5;           // Now it's (1, 5, 0)
    //   move.x += 2;          // Now it's (3, 5, 0)
    //
    // COMMON OPERATIONS:
    //   Vector3 a = new Vector3(1, 0, 1);
    //   Vector3 b = new Vector3(2, 0, 0);
    //   
    //   Vector3 sum = a + b;           // Adds: (3, 0, 1)
    //   Vector3 diff = a - b;          // Subtracts: (-1, 0, 1)
    //   Vector3 scaled = a * 5f;       // Multiplies each component: (5, 0, 5)
    //   
    //   float length = a.magnitude;    // Distance from origin: ~1.41
    //   a.Normalize();                 // Makes length = 1 (useful for directions!)
    //                                  // After: (~0.707, 0, ~0.707)
    //
    // REAL WORLD EXAMPLE IN YOUR CODE:
    //   // Get input values
    //   float horizontal = Input.GetAxis("Horizontal");  // -1 to 1
    //   float vertical = Input.GetAxis("Vertical");      // -1 to 1
    //   
    //   // Create movement vector
    //   Vector3 moveDirection = new Vector3(horizontal, 0, vertical);
    //   // This means:
    //   // - horizontal input moves us left/right (x)
    //   // - vertical input moves us forward/back (z)
    //   // - y stays 0 (we're on the ground, no up/down movement yet)
    //   
    //   // If player presses D and W:
    //   // horizontal = 1.0 (right)
    //   // vertical = 1.0 (forward)
    //   // moveDirection = (1, 0, 1) - moves diagonally!
    
    // ============================================================================
    // USEFUL UNITY FUNCTIONS & METHODS YOU'LL WANT TO USE:
    // ============================================================================
    
    // INPUT:
    // - Input.GetAxis("Horizontal") - Returns -1 to 1 for A/D or Left/Right arrows
    // - Input.GetAxis("Vertical") - Returns -1 to 1 for W/S or Up/Down arrows
    // - Input.GetButtonDown("Jump") - Returns true when Space is pressed
    // - Input.GetKey(KeyCode.LeftShift) - Returns true while Shift is held
    
    // CHARACTER CONTROLLER:
    // - characterController.Move(Vector3 direction) - Moves the character
    // - characterController.isGrounded - Returns true if touching ground
    // - characterController.height - Height of the character capsule
    
    // VECTORS (Vector3):
    // - Vector3.zero - Shorthand for (0, 0, 0)
    // - Vector3.up - Shorthand for (0, 1, 0)
    // - Vector3.down - Shorthand for (0, -1, 0)
    // - Vector3.forward - Shorthand for (0, 0, 1)
    // - Vector3.right - Shorthand for (1, 0, 0)
    // - vector.magnitude - Length/distance of the vector
    // - vector.Normalize() - Makes vector length 1 (useful for consistent speed)
    // - Vector3.Lerp(a, b, t) - Smoothly interpolates between two vectors
    // - Vector3.Distance(a, b) - Calculates distance between two points
    
    // TIME:
    // - Time.deltaTime - Time since last frame (for frame-rate independent movement)
    
    // PHYSICS:
    // - Physics.Raycast(origin, direction, distance) - Casts a ray to detect collisions
    //   Useful for ground detection!
    
    // TRANSFORM (this.transform or just transform):
    // - transform.position - Current position
    // - transform.rotation - Current rotation
    // - transform.forward - Forward direction (blue arrow in Unity)
    // - transform.right - Right direction (red arrow)
    
    // ============================================================================
    // FEATURES YOU CAN ADD (in order of difficulty):
    // ============================================================================
    
    // 1. JUMPING (Easy)
    //    - Add a float variable for jumpHeight or jumpForce
    //    - Add a float variable for gravity (something like -9.81f or -20f)
    //    - Add a Vector3 velocity variable to track vertical movement
    //    - In Update(), check Input.GetButtonDown("Jump") and if grounded
    //    - Set velocity.y = jumpForce
    //    - Apply gravity: velocity.y += gravity * Time.deltaTime
    //    - Include velocity in your Move() call
    
    // 2. GROUND DETECTION (Easy-Medium)
    //    - Use characterController.isGrounded (simple but works)
    //    - OR use Physics.Raycast downward to check for ground
    //    - Create a LayerMask to specify what counts as "ground"
    
    // 3. SPRINTING (Easy)
    //    - Add a float variable for sprintSpeed
    //    - Check if Shift key is held (Input.GetKey(KeyCode.LeftShift))
    //    - Use sprintSpeed instead of speed when sprinting
    
    // 4. CAMERA-RELATIVE MOVEMENT (Medium)
    //    - Add a public Transform cameraTransform variable
    //    - In Awake(), find Camera.main.transform if not assigned
    //    - Use camera's forward/right vectors instead of world space
    //    - Flatten them to XZ plane (set y to 0 and Normalize())
    
    // 5. SMOOTH ROTATION (Medium)
    //    - Make player face movement direction
    //    - Use Quaternion.LookRotation(direction)
    //    - Use Quaternion.Slerp() or Quaternion.Lerp() for smooth rotation
    
    // 6. INPUT SYSTEM (Advanced - Optional)
    //    - Use Unity's new Input System instead of old Input class
    //    - Requires InputActionAsset setup
    //    - More flexible but more complex to set up
}
