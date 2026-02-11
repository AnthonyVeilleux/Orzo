using System;
using UnityEngine;

public class Grab : MonoBehaviour
{
    [Header("Components")]
public Transform target; // The target object we picked up for scaling

[Header("Parameters")]
 public LayerMask targetMask;// The layer mask used to hit only potential targets with a raycast
 public LayerMask ignoreTargetMask; // The layer mask used to ignore the player and target objects while raycasting
 public float offsetFactor;// The offset amount for positioning the object so it doesn't clip into walls
 [SerializeField] private float additionalCameraOffset = 0.2f; // Extra offset to keep the object a bit closer to the camera
 [SerializeField] private float rotationSensitivity = 0.5f; // Sensitivity for object rotation
 float originalDistance; // The original distance between the player camera and the target
 Vector3 originalScale; // The original scale of the target objects prior to being resized
 
    private bool wasGrabHeld;
    private Rigidbody targetRigidbody;
     private Quaternion rotationOffset;

    public void Superliminal(bool grabHeld, bool rotateHeld, Vector2 mouseDelta)
    {
        if (grabHeld && !wasGrabHeld)
        {
            if (target == null)
            {
                TryGrab();
            }
            else
            {
                Release();
            }
        }

        if (target != null)
        {
            if (rotateHeld)
            {
                RotateTarget(mouseDelta);
            }
            ResizeTarget();
        }

        wasGrabHeld = grabHeld;
    }

    private void TryGrab()
    {
        if (Camera.main == null)
        {
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity, targetMask))
        {
            target = hit.transform;
            if (target.TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = true;
                targetRigidbody = rb;
            }

            rotationOffset = Quaternion.Inverse(Camera.main.transform.rotation) * target.rotation;

            originalDistance = Vector3.Distance(Camera.main.transform.position, target.position);
            originalScale = target.localScale;
        }
    }

    private void Release()
    {
        if (targetRigidbody != null)
        {
            targetRigidbody.isKinematic = false;
        }

        targetRigidbody = null;
        target = null;
    }

    private void RotateTarget(Vector2 mouseDelta)
    {
        // Rotate around camera's up axis (Y) with horizontal mouse movement
        float yaw = mouseDelta.x * rotationSensitivity;
        // Rotate around camera's right axis (X) with vertical mouse movement  
        float pitch = mouseDelta.y * rotationSensitivity;

        // Apply rotation relative to camera view
        rotationOffset *= Quaternion.Euler(-pitch, yaw, 0);
    }

    private void ResizeTarget()
    {
        if (Camera.main == null)
        {
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity, ignoreTargetMask))
        {
            target.position = hit.point - Camera.main.transform.forward * (offsetFactor + additionalCameraOffset) * target.localScale.x;
            target.rotation = Camera.main.transform.rotation * rotationOffset;
            // Find out if its colliding with anything, if so, move it back until it isn't
            // while (Physics.CheckBox(target.position, target.localScale / 2, target.rotation, ignoreTargetMask))
            // {
            //     target.position -= Camera.main.transform.forward * 0.02f;
            //     Debug.Log("Object is colliding, moving back" + target.position);
            // }
            float currentDistance = Vector3.Distance(Camera.main.transform.position, target.position);
            float s = currentDistance / originalDistance;

            target.localScale = originalScale * s;
        }
    }
}