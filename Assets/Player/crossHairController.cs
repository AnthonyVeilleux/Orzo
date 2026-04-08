using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class crossHairController : MonoBehaviour
{
    public LayerMask targetMask;
    public UnityEngine.UI.Image crosshairImage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
RaycastHit hit;

if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity))
{
    if (((1 << hit.collider.gameObject.layer) & targetMask) != 0)
    {
        crosshairImage.color = Color.green; // actual target
    }
    else
    {
        crosshairImage.color = Color.white; // hit something else first
    }
}
else
{
    crosshairImage.color = Color.white;
}
    }
}
