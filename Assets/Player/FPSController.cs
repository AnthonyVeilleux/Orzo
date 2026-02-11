using System;
using UnityEngine;
public class FPSController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 2f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Camera Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 90f;
    private CharacterController characterController;
    private Camera mainCamera;
    private PlayerInputHandler inputHandler;
    private Vector3 currentMovement;
    private float verticalRotation;
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        inputHandler = PlayerInputHandler.instance;
        if (inputHandler == null)
        {
            inputHandler = FindObjectOfType<PlayerInputHandler>();
        }
    }
    private void Update()
    {
        if (characterController == null || inputHandler == null || mainCamera == null)
        {
            return;
        }
        HandleMovement();
        HandleJumping();
        HandleRotation();
        HandleGrabbing();
    }

    private void HandleMovement()
    {
        float speed = walkSpeed * (inputHandler.SprintInput ? sprintMultiplier : 1f);
        UnityEngine.Vector3 inputDirection = new UnityEngine.Vector3(inputHandler.MoveInput.x, 0, inputHandler.MoveInput.y);
        UnityEngine.Vector3 worldDirection = transform.TransformDirection(inputDirection).normalized;
        currentMovement.x = worldDirection.x* speed;
        currentMovement.z = worldDirection.z* speed;

        characterController.Move(currentMovement * Time.deltaTime);

}
    void HandleJumping(){
        if (characterController.isGrounded && inputHandler.JumpInput)
        {
            currentMovement.y = jumpForce;
        }
        currentMovement.y += gravity * Time.deltaTime;
    }
    void HandleRotation(){
        // Don't rotate camera when rotating grabbed object
        if (inputHandler.RotateInput)
        {
            return;
        }

        float mouseX = inputHandler.LookInput.x * mouseSensitivity;
        float mouseY = inputHandler.LookInput.y * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
        mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);

        transform.Rotate(0f, mouseX, 0f);

    }
    void HandleGrabbing(){
        Grab grabComponent = GetComponent<Grab>();
        if(grabComponent != null){
            grabComponent.Superliminal(inputHandler.GrabInput, inputHandler.RotateInput, inputHandler.LookInput);
        }
    }
}