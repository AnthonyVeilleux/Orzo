using System;
using UnityEngine;

/// <summary>
/// Handles picking up, scaling, and rotating objects in a "Superliminal" style mechanic.
/// Objects appear to change size based on distance from camera (forced perspective).
/// </summary>
public class Grab : MonoBehaviour
{
    // ========== INSPECTOR FIELDS ==========
    [Header("Components")]
    public Transform target; // Reference to the currently grabbed object's Transform

    [Header("Parameters")]
    public LayerMask targetMask; // Which layers can be grabbed (e.g., "Grabbable" layer)
    public LayerMask ignoreTargetMask; // Which layers to ignore when raycasting (e.g., player, UI)
    public float offsetFactor; // Base offset to prevent object from clipping into walls
    [SerializeField] private float additionalCameraOffset = 0.2f; // Extra padding distance
    [SerializeField] private float rotationSensitivity = 0.5f; // How fast object rotates with mouse
     [SerializeField] private float distanceFromCamera = 0.2f; // How far from the camera the object is initialy placed before being dropped and then rescaled.
    [SerializeField] private Collider playerCollider; // Reference to player's collider to ignore collision while holding
    [SerializeField] private float releaseStepSize = 0.05f; // How far to move object each step when releasing
    [SerializeField] private float collisionPullback = 0.1f; // How far to pull back from collision point

    // ========== MEMORY VARIABLES ==========
    // These store the object's state when first grabbed
    float originalDistance; // Camera-to-object distance at moment of pickup
    Vector3 originalScale; // Object's original scale before any resizing
    
    
    // ========== STATE TRACKING ==========
    private bool wasGrabHeld; // Was the grab button pressed last frame? (for edge detection)
    private Rigidbody targetRigidbody; // Cache of the grabbed object's rigidbody component
    private Quaternion rotationOffset; // How rotated is the object relative to camera when grabbed?
    private Collider targetCollider; // Cache of the grabbed object's collider component


    /// <summary>
    /// Main update function called every frame.
    /// </summary>
    /// <param name="grabHeld">Is the grab button currently pressed?</param>
    /// <param name="rotateHeld">Is the rotate button currently pressed?</param>
    /// <param name="mouseDelta">How much did the mouse move this frame?</param>
    public void Superliminal(bool grabHeld, bool rotateHeld, Vector2 mouseDelta)
    {
        // ========== GRAB BUTTON LOGIC (EDGE DETECTION) ==========
        // Check if grab button was JUST pressed (not held from previous frame)
        if (grabHeld && !wasGrabHeld)
        {
            // Toggle behavior: if nothing held, try to grab; if holding something, release it
            if (target == null)
            {
                TryGrab(); // Attempt to pick up object under crosshair
            }
            else
            {
                Release(); // Drop currently held object
            }
        }

        // ========== WHILE HOLDING AN OBJECT ==========
        if (target != null)
        {
            // If rotate button held, apply mouse rotation to object
            if (rotateHeld)
            {
                RotateTarget(mouseDelta);
            }
            
            // ========== UPDATE POSITION WITH CAMERA ==========
            target.position = Camera.main.transform.position + Camera.main.transform.forward * distanceFromCamera;

            // ========== UPDATE ROTATION WITH CAMERA ==========
            target.rotation = Camera.main.transform.rotation * rotationOffset;

            // ========== UPDATE SCALE BASED ON HOLD DISTANCE ==========
            float scaleFactor = distanceFromCamera / originalDistance;
            target.localScale = originalScale * scaleFactor;
        }

        // ========== REMEMBER STATE FOR NEXT FRAME ==========
        // Store current grab state so we can detect button press edges next frame
        wasGrabHeld = grabHeld;
    }

    /// <summary>
    /// Attempts to pick up an object by raycasting forward from camera.
    /// Only runs when grab button is first pressed and no object is held.
    /// </summary>
    private void TryGrab()
    {
        // Safety check: make sure main camera exists
        if (Camera.main == null)
        {
            return;
        }

        // ========== RAYCAST TO FIND OBJECT ==========
        RaycastHit hit; // Stores information about what the ray hit
        
        // Shoot a ray from camera position, in camera forward direction, infinite distance, only hitting targetMask layers
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity, targetMask))
        {
            // ========== RAY HIT SOMETHING WE CAN GRAB ==========
            target = hit.transform; // Store reference to the object's transform
            targetCollider = hit.collider; // Cache the collider for later use (ignoring collision with player)
            // ========== DISABLE PHYSICS ON GRABBED OBJECT ==========
            // Check if object has a Rigidbody component
            if (target.TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = true; // Make it kinematic (we control position manually, physics won't affect it)
                targetRigidbody = rb; // Cache the rigidbody for later
            }

            // ========== IGNORE COLLISION WITH PLAYER ==========
            // Prevent grabbed object from colliding with player while held
            if (playerCollider != null && target.TryGetComponent(out Collider col))
            {
                Physics.IgnoreCollision(col, playerCollider, true); // Ignore collision between target and player
                targetCollider = col; // Cache the collider for later
            }

            // ========== STORE ROTATION OFFSET ==========
            // Calculate how rotated the object is relative to the camera
            // Quaternion.Inverse gets the "opposite" rotation of the camera
            // Multiplying by target.rotation gives us the difference between camera and object rotation
            // This lets us preserve the object's rotation relationship to camera when rotating
            rotationOffset = Quaternion.Inverse(Camera.main.transform.rotation) * target.rotation;

            // ========== STORE ORIGINAL VALUES FOR SCALING ==========
            // Remember how far away object was when grabbed (needed for proportional scaling)
            originalDistance = Vector3.Distance(Camera.main.transform.position, target.position);
            
            // Remember original scale BEFORE any scaling
            originalScale = target.localScale;


    
        }
    }

    /// <summary>
    /// Releases the currently held object, re-enabling physics.
    /// Moves object away from camera step-by-step until collision is detected.
    /// </summary>
    private void Release()
    {
        if (target == null)
        {
            return;
        }

        // ========== RESTORE COLLISION WITH PLAYER ==========
        // Re-enable collision between target and player
        if (playerCollider != null && targetCollider != null)
        {
            Physics.IgnoreCollision(targetCollider, playerCollider, false); // Restore collision
        }

        // ========== MOVE AWAY FROM CAMERA UNTIL COLLISION ==========
        Vector3 cameraPos = Camera.main.transform.position;
        Vector3 cameraForward = Camera.main.transform.forward;
        float currentDistance = distanceFromCamera;
        
        // Track last valid position
        Vector3 lastValidPosition = target.position;
        Vector3 lastValidScale = target.localScale;
        
        // Move step-by-step forward until collision
        int maxSteps = 1000; // Safety limit to prevent infinite loop
        for (int i = 0; i < maxSteps; i++)
        {
            // Try moving forward one step
            currentDistance += releaseStepSize;
            
            // Calculate new position and scale
            Vector3 newPosition = cameraPos + cameraForward * currentDistance;
            float scaleFactor = currentDistance / originalDistance;
            Vector3 newScale = originalScale * scaleFactor;
            
            // Apply new position and scale
            target.position = newPosition;
            target.localScale = newScale;
            
            // Force physics update to recalculate bounds
            Physics.SyncTransforms();
            
            // Check for collision using actual collider
            Collider[] hits = Physics.OverlapBox(
                targetCollider.bounds.center,
                targetCollider.bounds.extents,
                target.rotation,
                ignoreTargetMask
            );
            
            // Check if we hit anything other than ourselves
            bool hitSomething = false;
            foreach (Collider hit in hits)
            {
                if (hit != targetCollider)
                {
                    Debug.Log("Collision detected with: " + hit.gameObject.name);
                    hitSomething = true;
                    break;
                }
            }
            
            if (hitSomething)
            {
                // Collision detected - move back to last valid position and pull back a bit more
                float finalDistance = currentDistance - releaseStepSize - collisionPullback;
                target.position = cameraPos + cameraForward * finalDistance;
                float finalScaleFactor = finalDistance / originalDistance;
                target.localScale = originalScale * finalScaleFactor;
                break;
            }
            
            // Update last valid position
            lastValidPosition = newPosition;
            lastValidScale = newScale;
        }

        // ========== RE-ENABLE PHYSICS ==========
        if (targetRigidbody != null)
        {
            targetRigidbody.isKinematic = false; // Let physics take over again (object will fall, collide, etc)
        }

        // ========== CLEAR REFERENCES ==========
        targetRigidbody = null;
        targetCollider = null;
        target = null;
    }

    /// <summary>
    /// Rotates the held object based on mouse movement.
    /// Only runs when rotate button is held.
    /// </summary>
    /// <param name="mouseDelta">Mouse movement this frame (x = horizontal, y = vertical)</param>
    private void RotateTarget(Vector2 mouseDelta)
    {
        // ========== CONVERT MOUSE MOVEMENT TO ROTATION ==========
        // Horizontal mouse movement (left/right) rotates around the vertical axis (yaw)
        float yaw = mouseDelta.x * rotationSensitivity;
        
        // Vertical mouse movement (up/down) rotates around the horizontal axis (pitch)
        float pitch = mouseDelta.y * rotationSensitivity;

        // ========== ACCUMULATE ROTATION ==========
        // Apply the rotation to our stored rotationOffset
        // Quaternion.Euler creates a rotation from euler angles (pitch, yaw, roll)
        // Using *= means we're adding this rotation on top of previous rotations
        // Negative pitch because mouse up should rotate up
        rotationOffset *= Quaternion.Euler(-pitch, yaw, 0);
    }

    /// <summary>
    /// Updates the held object's position and scale every frame.
    /// This is what creates the "Superliminal" forced perspective effect.
    /// </summary>
    private void ResizeTarget()
    {
        // Safety check
        if (Camera.main == null)
        {
            return;
        }

        // ========== RAYCAST TO FIND WHERE TO PLACE OBJECT ==========
        RaycastHit hit;
        
        // Shoot ray from camera forward, ignoring layers in ignoreTargetMask
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity, ignoreTargetMask))
        {
            // ========== POSITION OBJECT AT RAY HIT POINT ==========
            // Place object at the point the ray hit the world
            // Subtract (camera forward × offset × object scale) to pull object back from wall
            // This prevents clipping into walls
            // Using target.localScale.x as a rough measure of object size
            target.position = hit.point - Camera.main.transform.forward * (offsetFactor + additionalCameraOffset) * target.localScale.x;
            
            // ========== APPLY ROTATION ==========
            // Rotate object to match camera rotation, preserving the original rotation offset
            target.rotation = Camera.main.transform.rotation * rotationOffset;
            
            // ========== COMMENTED OUT COLLISION DETECTION ==========
            // This code was disabled - it would check if object is clipping and move it back
            while (Physics.CheckBox(target.position, target.localScale / 2, target.rotation, ignoreTargetMask))
            {
                target.position -= Camera.main.transform.forward * 0.02f;
                Debug.Log("Object is colliding, moving back" + target.position);
            }
            
            // ========== CALCULATE NEW SCALE (THE MAGIC) ==========
            // This creates the forced perspective illusion
            
            // Calculate current distance from camera to object
            float currentDistance = Vector3.Distance(Camera.main.transform.position, target.position);
            
            // Calculate scale factor: ratio of current distance to original distance
            // If object is now 2x farther away, scale factor = 2.0 (object gets 2x bigger)
            // If object is now 0.5x as far away, scale factor = 0.5 (object gets 2x smaller)
            float s = currentDistance / originalDistance;

            // Apply the scale factor to the original scale
            // This makes objects appear the same size on screen regardless of distance
            target.localScale = originalScale * s;
        }
    }
}