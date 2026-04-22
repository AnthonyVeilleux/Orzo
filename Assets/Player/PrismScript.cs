using UnityEngine;

public class PrismScript : MonoBehaviour
{
    public bool debugAlwaysOn = false;
    public float maxDistance = 100f;
    public GameObject respawnPoint;
    public bool isDeadly = true;

    private LineRenderer lr;
    private bool _hitThisFrame = false;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        if (lr != null)
        {
            lr.useWorldSpace = true;
            lr.enabled = false;
        }
    }

    // Called by laserScript each frame this prism is hit by a laser
    public void NotifyHit()
    {
        _hitThisFrame = true;
    }

    void LateUpdate()
    {
        bool shouldBeOn = debugAlwaysOn || _hitThisFrame;
        _hitThisFrame = false;

        if (lr == null) return;

        lr.enabled = shouldBeOn;

        if (!shouldBeOn) return;

        Vector3 start = transform.position;
        Vector3 direction = transform.forward;

        lr.SetPosition(0, start);

        RaycastHit hit;
        if (Physics.Raycast(start, direction, out hit, maxDistance))
        {
            lr.SetPosition(1, hit.point);

            if (hit.collider.CompareTag("Player") && isDeadly && respawnPoint != null)
            {
                Teleport.TeleportTo(hit.collider.gameObject, respawnPoint.transform.position, respawnPoint.transform.rotation);
            }

            LaserCheck laserCheck = hit.collider.GetComponent<LaserCheck>();
            if (laserCheck != null)
            {
                laserCheck.NotifyHitByPrismLaser();
            }
        }
        else
        {
            lr.SetPosition(1, start + direction * maxDistance);
        }
    }
}
