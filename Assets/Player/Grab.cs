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
 float originalDistance; // The original distance between the player camera and the target
 float originalScale; // The original scale of the target objects prior to being resized
 
 Vector3 targetScale; // The scale we want our object to be set to each frame
    private bool wasGrabHeld;
    private Rigidbody targetRigidbody;
     private Quaternion rotationOffset;

    public void Superliminal(bool grabHeld)
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
            originalScale = target.localScale.x;
            targetScale = target.localScale;
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

    private void ResizeTarget()
    {
        if (Camera.main == null)
        {
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity, ignoreTargetMask))
        {
            target.position = hit.point - Camera.main.transform.forward * (offsetFactor + additionalCameraOffset) * targetScale.x;
            target.rotation = Camera.main.transform.rotation * rotationOffset;

            float currentDistance = Vector3.Distance(Camera.main.transform.position, target.position);
            float s = currentDistance / originalDistance;

            targetScale.x = targetScale.y = targetScale.z = s;
            target.localScale = targetScale * originalScale;
        }
    }
}