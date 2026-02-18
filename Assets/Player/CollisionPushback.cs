using UnityEngine;

/// <summary>
/// Handles pushing objects back toward camera when they collide during release.
/// Attach this to grabbable objects.
/// </summary>
public class CollisionPushback : MonoBehaviour
{
    [SerializeField] private float pushbackMultiplier = 1.5f; // How much to push back from collision
    [SerializeField] private float pushbackStepSize = 0.05f; // Distance to push back per collision
    
    private bool isBeingReleased = false;
    private Vector3 cameraPosition;
    private Vector3 cameraForward;
    private float originalDistance;
    private Vector3 originalScale;
    
    /// <summary>
    /// Call this when starting to release the object to enable collision pushback.
    /// </summary>
    public void StartRelease(Vector3 camPos, Vector3 camForward, float origDistance, Vector3 origScale)
    {
        isBeingReleased = true;
        cameraPosition = camPos;
        cameraForward = camForward;
        originalDistance = origDistance;
        originalScale = origScale;
    }
    
    /// <summary>
    /// Call this when release is complete to disable collision pushback.
    /// </summary>
    public void EndRelease()
    {
        isBeingReleased = false;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (!isBeingReleased)
        {
            return;
        }
        
        // Object collided during release - push it back toward camera
        Debug.Log("Collision detected with: " + collision.gameObject.name + " - pushing back");
        
        // Calculate current distance from camera
        float currentDistance = Vector3.Distance(cameraPosition, transform.position);
        
        // Push back toward camera
        float newDistance = currentDistance - (pushbackStepSize * pushbackMultiplier);
        transform.position = cameraPosition + cameraForward * newDistance;
        
        // Update scale to match new position (maintain forced perspective)
        float scaleFactor = newDistance / originalDistance;
        transform.localScale = originalScale * scaleFactor;
    }
}
