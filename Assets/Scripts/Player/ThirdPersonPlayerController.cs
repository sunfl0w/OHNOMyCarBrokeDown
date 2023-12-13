using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonPlayerController : MonoBehaviour {
    public GameObject mainCamera;
    public Animator animator;
    public float walkSpeed = 2.2f;
    public float runSpeed = 6.0f;
    public float gravity = 10f;
    public float rotationSpeed = 20.0f;

    Vector3 moveVector = Vector3.zero;
    Vector3 targetMoveDirection = Vector3.zero;
    public bool canMove = true;
    private bool isRunning = false;
    public float currentSpeed = 0.0f;
    public float translationDampening = 3.0f;
    public float rotationDampening = 5.0f;

    CameraController camController;
    CharacterController characterController;

    void Awake() {
        // Add callbacks to GUI events
        ItemInspectGUI.onInspectionGUIEnter += DisableMovement;
        ItemInspectGUI.onInspectionGUILeave += EnableMovement;
    }

    void Start() {
        characterController = GetComponent<CharacterController>();
        camController = mainCamera.GetComponent<CameraController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void FixedUpdate() {
        // Press Left Shift to run
        //isRunning = Input.GetKey(KeyCode.LeftShift);

        // "Tank-Control" player movement
        targetMoveDirection = ((transform.forward * Input.GetAxisRaw("Vertical")) + (transform.right * Input.GetAxisRaw("Horizontal"))).normalized;
        float targetSpeed = canMove && targetMoveDirection.magnitude > 0.0f ? (isRunning && GetIsGrounded() ? runSpeed : walkSpeed) : 0;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * translationDampening);
        if (targetMoveDirection.magnitude > 0.0f && canMove) { // Only rotate if the player inputs a movement direction
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetMoveDirection, Vector3.up), Time.deltaTime * rotationDampening);
        }
        animator.SetFloat("movementSpeed", currentSpeed);

        moveVector = transform.forward * currentSpeed;
        characterController.Move((moveVector + Vector3.down * gravity) * Time.deltaTime);
    }

    public bool GetIsRunning() {
        return isRunning;
    }

    public bool GetIsMoving() {
        return moveVector.x * moveVector.x + moveVector.z * moveVector.z > 0.1f;
    }

    public bool GetIsGrounded() {
        return characterController.isGrounded;
    }

    public void DisableMovement() {
        canMove = false;
    }

    public void EnableMovement() {
        canMove = true;
    }
}
