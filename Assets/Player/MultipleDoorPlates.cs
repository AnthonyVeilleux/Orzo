using System.Collections;
using UnityEngine;

public class MultipleDoorPlates : MonoBehaviour
{
    public PressurePlate[] pressurePlates; // Array of pressure plates that need to be active
    public Vector3 doorClosedPosition = Vector3.zero; // Relative offset when door is closed
    public Vector3 doorOpenPosition = Vector3.up * 2f; // Relative offset when door is open
    public float doorLerpDuration = 1f; // How long the door takes to open/close

    private bool doorState = false; // Track the state of the door (open or closed)
    private Coroutine doorCoroutine;
    private Vector3 doorSpawnPosition; // Initial position of the door

    void Start()
    {
        doorSpawnPosition = transform.position;
    }

    void Update()
    {
        // Check if all pressure plates are active
        bool allPlatesActive = true;
        foreach (PressurePlate plate in pressurePlates)
        {
            if (plate.objectsOnPlate <= 0)
            {
                allPlatesActive = false;
                break;
            }
        }

        // Open door if all plates active, close if not
        if (allPlatesActive && !doorState)
        {
            OpenDoor();
        }
        else if (!allPlatesActive && doorState)
        {
            CloseDoor();
        }
    }

    private void OpenDoor()
    {
        if (doorCoroutine != null)
        {
            StopCoroutine(doorCoroutine);
        }
        doorCoroutine = StartCoroutine(LerpDoor(doorSpawnPosition + doorOpenPosition));
        doorState = true;
    }

    private void CloseDoor()
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
        Vector3 startPos = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < doorLerpDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / doorLerpDuration);
            transform.position = Vector3.Lerp(startPos, targetPosition, t);
            yield return null;
        }

        transform.position = targetPosition;
    }
}
