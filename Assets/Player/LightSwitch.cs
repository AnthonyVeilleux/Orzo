using UnityEngine;

/// <summary>
/// Place on a light-switch object. When the player presses Grab while looking at it
/// (and not already holding something), toggles the reveal shader between full-reveal
/// and normal radius mode.
/// </summary>
public class LightSwitch : MonoBehaviour
{
    [Header("Reveal Material")]
    public Material revealMaterial; // The same material used by RevealLightController

    [Header("Settings")]
    [SerializeField] private float interactDistance = 3f;  // How far away the player can flip the switch
    [SerializeField] private float normalRadius = 2f;       // Radius when lights are OFF
    [SerializeField] private float revealAllRadius = 9999f; // Radius when lights are ON (reveals everything)

    private bool lightsOn = false;
    private bool wasGrabHeld = false;
    private Grab grabComponent;
    private float originalRadius;

    void Start()
    {
        grabComponent = FindAnyObjectByType<Grab>();
        originalRadius = revealMaterial.GetFloat("_RevealRadius");
    }

    void OnDestroy()
    {
        // Restore the material to its original state so scene resets don't carry over changes
        revealMaterial.SetFloat("_RevealRadius", originalRadius);
    }

    void Update()
    {
        if (grabComponent == null || Camera.main == null)
            return;

        PlayerInputHandler input = PlayerInputHandler.instance;
        if (input == null)
            return;

        bool grabPressed = input.GrabInput && !wasGrabHeld; // edge detect
        wasGrabHeld = input.GrabInput;

        // Only activate when grab is just pressed, player isn't holding anything, and is looking at this switch
        if (grabPressed && grabComponent.target == null)
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward,
                out RaycastHit hit, interactDistance) && hit.transform == transform)
            {
                Toggle();
            }
        }
    }

    void Toggle()
    {
        lightsOn = !lightsOn;
        revealMaterial.SetFloat("_RevealRadius", lightsOn ? revealAllRadius : normalRadius);
    }
}
