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
    if (!obj.CompareTag("Player"))
        return;

    Transform player = obj.transform;

    // compute player position relative to teleport trigger
    Vector3 relativePos = transform.InverseTransformPoint(player.position);

    // move player relative to destination
    Vector3 newPos = teleportLocation.transform.TransformPoint(relativePos);

    // compute relative rotation
    Quaternion relativeRot =
        Quaternion.Inverse(transform.rotation) * player.rotation;

    Quaternion newRot =
        teleportLocation.transform.rotation * relativeRot;

    TeleportTo(obj, newPos, newRot);
}

    public static void TeleportTo(GameObject obj, Vector3 targetPos, Quaternion targetRot)
    {
        Transform player = obj.transform;
        CharacterController cc = obj.GetComponent<CharacterController>();

        if (cc != null)
            cc.enabled = false;

        player.position = targetPos;
        player.rotation = targetRot;

        if (cc != null)
            cc.enabled = true;
    }
}
