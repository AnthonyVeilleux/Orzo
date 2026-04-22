using UnityEngine;

public class laserScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private LineRenderer lr;
    public float maxDistance = 100f;
    public GameObject respawnPoint;
    public bool isDeadly = true;
    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        Vector3 start = transform.position;
        Vector3 direction = transform.forward;

        lr.SetPosition(0, start);

        RaycastHit hit;
        if (Physics.Raycast(start, direction, out hit, maxDistance))
        {
            lr.SetPosition(1, hit.point);
            //debug if we hit the player or not
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("Hit: " + hit.collider.name);
                if (isDeadly)
                {
                    //teleport the player to the respawn point
                    Teleport.TeleportTo(hit.collider.gameObject, respawnPoint.transform.position, respawnPoint.transform.rotation);
                }
            }
            else if (hit.collider.CompareTag("Prism"))
            {
                PrismScript prism = hit.collider.GetComponent<PrismScript>();
                if (prism != null)
                    prism.NotifyHit();
            }
            
        }
        else
        {
            lr.SetPosition(1, start + direction * maxDistance);
        }
    }

}
