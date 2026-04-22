using System.Collections;
using UnityEngine;

public class LaserCheck : MonoBehaviour
{
    public GameObject door; // Door to animate when laser is hitting this checker
    public float doorLerpDuration = 1f;
    public Vector3 doorClosedPosition = Vector3.zero;
    public Vector3 doorOpenPosition = Vector3.up * 2f;

    private bool doorState = false;
    private bool hitThisFrame = false;
    private Coroutine doorCoroutine;
    private Vector3 doorSpawnPosition;

    private void Start()
    {
        if (door != null)
        {
            doorSpawnPosition = door.transform.position;
        }
    }

    // Called by PrismScript when the prism laser ray is currently hitting this object.
    public void NotifyHitByPrismLaser()
    {
        hitThisFrame = true;
    }

    private void LateUpdate()
    {
        if (door == null)
        {
            hitThisFrame = false;
            return;
        }

        if (hitThisFrame)
        {
            if (!doorState)
            {
                MoveDoorOpenDirection();
            }
        }
        else
        {
            if (doorState)
            {
                MoveDoorCloseDirection();
            }
        }

        hitThisFrame = false;
    }

    private void MoveDoorOpenDirection()
    {
        if (doorCoroutine != null)
        {
            StopCoroutine(doorCoroutine);
        }

        doorCoroutine = StartCoroutine(LerpDoor(doorSpawnPosition + doorOpenPosition));
        doorState = true;
    }

    private void MoveDoorCloseDirection()
    {
        if (doorCoroutine != null)
        {
            StopCoroutine(doorCoroutine);
        }

        doorCoroutine = StartCoroutine(LerpDoor(doorSpawnPosition + doorClosedPosition));
        doorState = false;
    }

    private IEnumerator LerpDoor(Vector3 targetPosition)
    {
        Vector3 startPos = door.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < doorLerpDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / doorLerpDuration);
            door.transform.position = Vector3.Lerp(startPos, targetPosition, t);
            yield return null;
        }

        door.transform.position = targetPosition;
    }
}
