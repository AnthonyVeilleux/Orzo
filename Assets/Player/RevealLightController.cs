using UnityEngine;

public class RevealLightController : MonoBehaviour
{
    public Material revealMaterial;

    void Update()
    {
        revealMaterial.SetVector("_RevealLightPos", transform.position);
    }
}