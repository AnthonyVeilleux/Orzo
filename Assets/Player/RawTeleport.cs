using UnityEngine;

public class RawTeleport : MonoBehaviour
{
    [SerializeField] private GameObject  teleportLocation; // The location to teleport the player to
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if (teleportLocation != null)
        {
            Teleport.TeleportTo(other.gameObject, teleportLocation.transform.position, teleportLocation.transform.rotation);
        }
    }
}
