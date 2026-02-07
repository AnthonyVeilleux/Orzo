using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Player Input Handler")]
    [SerializeField] private InputActionAsset playerInput;

    [Header("Action Map Name References")]
    [SerializeField] private string actionMapName = "Player";

    [Header("Input Action Name References")]
    [SerializeField] private string moveActionName = "Move";
    [SerializeField] private string lookActionName = "Look";
    [SerializeField] private string jumpActionName = "Jump";
    [SerializeField] private string sprintActionName = "Sprint";
    [SerializeField] private string grabActionName = "Grab";

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction grabAction;

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool JumpInput { get; private set; } 
    public bool SprintInput { get; private set; }
    public bool GrabInput { get; private set; }
    public static PlayerInputHandler instance { get; private set; }

    private void Awake()
    {
        if( instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        moveAction = playerInput.FindActionMap(actionMapName).FindAction(moveActionName);
        lookAction = playerInput.FindActionMap(actionMapName).FindAction(lookActionName);
        jumpAction = playerInput.FindActionMap(actionMapName).FindAction(jumpActionName);
        sprintAction = playerInput.FindActionMap(actionMapName).FindAction(sprintActionName);
        grabAction = playerInput.FindActionMap(actionMapName).FindAction(grabActionName);
        RegisterInputActions();
    }
    void RegisterInputActions()
    {
        moveAction.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        moveAction.canceled += ctx => MoveInput = Vector2.zero;

        lookAction.performed += ctx => LookInput = ctx.ReadValue<Vector2>();
        lookAction.canceled += ctx => LookInput = Vector2.zero;

        jumpAction.performed += ctx => JumpInput = true;
        jumpAction.canceled += ctx => JumpInput = false;

        sprintAction.performed += ctx => SprintInput = true;
        sprintAction.canceled += ctx => SprintInput = false;

        grabAction.performed += ctx => GrabInput = true;
        grabAction.canceled += ctx => GrabInput = false;
    }
    private void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
        jumpAction.Enable();
        sprintAction.Enable();
        grabAction.Enable();
    }
    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        jumpAction.Disable();
        sprintAction.Disable();
        grabAction.Disable();
    }
}