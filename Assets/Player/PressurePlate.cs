using System;
using System.Collections;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public GameObject door; // Reference to the door that will be opened when the pressure plate is stepped on
    public GameObject plateVisual; // Reference to the visual representation of the pressure plate
    public float doorLerpDuration = 1f; // How long the door takes to open/close
    public Vector3 doorClosedPosition = Vector3.zero; // Relative offset when door is closed
    public Vector3 doorOpenPosition = Vector3.up * 2f; // Relative offset when door is open

    private Boolean doorState = false; // Track the state of the door (open or closed)
    private Coroutine doorCoroutine;
    private Vector3 doorSpawnPosition; // Initial position of the door
    public int objectsOnPlate = 0; // Count of objects currently on plate

    private void Start()
    {
        // Cache the initial door position
        if (door != null)
        {
            doorSpawnPosition = door.transform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        objectsOnPlate++;
        Debug.Log(other.name + " is on the pressure plate!");
        plateVisual.transform.position += Vector3.down * 0.1f;
    }

    private void OnTriggerStay(Collider other)
    {
        // Door opens when objects are on the plate
        if (!doorState)
        {
            moveDoorOpenDirection();
        }
    }

    private void moveDoorOpenDirection()
    {
        // Stop any existing door movement
        if (doorCoroutine != null)
        {
            StopCoroutine(doorCoroutine);
        }
        doorCoroutine = StartCoroutine(LerpDoor(doorSpawnPosition + doorOpenPosition));
        doorState = true;
    }

    private void moveDoorCloseDirection()
    {
        // Stop any existing door movement
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

    private void OnTriggerExit(Collider other)
    {
        objectsOnPlate--;
        Debug.Log(other.name + " stepped off the pressure plate!");
        plateVisual.transform.position += Vector3.up * 0.1f;
        
        // Close door if no more objects on plate
        if (objectsOnPlate <= 0 && doorState)
        {
            objectsOnPlate = 0;
            moveDoorCloseDirection();
        }
    }
}
