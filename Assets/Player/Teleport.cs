using System;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

[SerializeField] private Vector3 teleportPosition = new Vector3(0, 1, 0); // Position to teleport the player to
[SerializeField] private GameObject  teleportLocation; // The location to teleport the player to
  

    private void OnCollisionEnter(Collision collision)
    {
        TeleportObject(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        TeleportObject(other.gameObject);
    }

    private void TeleportObject(GameObject obj)
    {
        if (obj.CompareTag("Player"))
        {
            CharacterController cc = obj.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
                obj.transform.position = teleportLocation.transform.position; // Teleport the player to a new position
                cc.enabled = true;
            }
            else
            {
                obj.transform.position = teleportLocation.transform.position; // Teleport the player to a new position
            }
        }
    }
}
