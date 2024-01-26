using UnityEngine;

/// <summary>
/// The third person player controller controls the player character based on user input.
/// </summary>
public class ThirdPersonPlayerController : MonoBehaviour {
    /// <summary>
    /// Main camera reference.
    /// </summary>
    public GameObject mainCamera;

    /// <summary>
    /// Animator reference.
    /// </summary>
    public Animator animator;

    /// <summary>
    /// Walking speed of the player character.
    /// </summary>
    public float walkSpeed = 2.2f;

    /// <summary>
    /// Gravity acceleration.
    /// </summary>
    public float gravity = 10f;

    /// <summary>
    /// Rotation speed of the player character.
    /// </summary>
    public float rotationSpeed = 20.0f;

    /// <summary>
    /// Current movement direction of the player character.
    /// </summary>
    Vector3 moveVector = Vector3.zero;

    /// <summary>
    /// Target movement direction of the player character.
    /// </summary>
    Vector3 targetMoveDirection = Vector3.zero;

    /// <summary>
    /// Specifies, whether the player character can move currently.
    /// </summary>
    public bool canMove = true;

    /// <summary>
    /// Specifies, whether the player character can move currently.
    /// </summary>
    public float currentSpeed = 0.0f;

    /// <summary>
    /// Translation dampening coefficient used for player movement.
    /// </summary>
    public float translationDampening = 3.0f;

    /// <summary>
    /// Rotation dampening coefficient used for player movement.
    /// </summary>
    public float rotationDampening = 5.0f;

    /// <summary>
    /// Specifies, whether the player is currently in transition to a new scene.
    /// </summary>
    private bool inTransition = false;

    /// <summary>
    /// Reference to the camera controller.
    /// </summary>
    CameraController camController;

    /// <summary>
    /// Reference to the character controller.
    /// </summary>
    CharacterController characterController;

    void Awake() {
        // Add callbacks to GUI events
        UnifiedGUI.GUIEnterEvent += OnGUIEnter;
        UnifiedGUI.GUILeaveEvent += EnableMovement;
    }

    void Start() {
        characterController = GetComponent<CharacterController>();
        camController = mainCamera.GetComponent<CameraController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void FixedUpdate() {
        // "Tank-Control" player movement
        targetMoveDirection = ((transform.forward * Mathf.Max(Input.GetAxisRaw("Vertical"), 0.0f)) + (transform.right * Input.GetAxisRaw("Horizontal"))).normalized;
        float targetSpeed = canMove && targetMoveDirection.magnitude > 0.0f ? walkSpeed : 0.0f;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * translationDampening);
        if (targetMoveDirection.magnitude > 0.0f && canMove) { // Only rotate if the player inputs a movement direction
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetMoveDirection, Vector3.up), Time.deltaTime * rotationDampening);
        }
        animator.SetFloat("movementSpeed", currentSpeed); // Update animator based on current speed so the animator can choose the correct animation state

        moveVector = transform.forward * currentSpeed;

        if (canMove) {
            characterController.Move((moveVector + Vector3.down * gravity) * Time.deltaTime);
        }
    }

    /// <summary>
    /// Returns true if the player character is moving.
    /// </summary>
    public bool GetIsMoving() {
        return moveVector.x * moveVector.x + moveVector.z * moveVector.z > 0.1f;
    }

    /// <summary>
    /// Returns true if the player character is grounded.
    /// </summary>
    public bool GetIsGrounded() {
        return characterController.isGrounded;
    }

    /// <summary>
    /// Is called when a gui is entered.
    /// </summary>
    public void OnGUIEnter(bool enableGUIBlur, bool disableMovement) {
        if (disableMovement) { // Diabale player movement if desired
            canMove = false;
            currentSpeed = 0.0f;
        }
    }

    /// <summary>
    /// Enables player movement.
    /// </summary>
    public void EnableMovement() {
        canMove = true;
    }

    /// <summary>
    /// Notifies the character controller of a scene transition.
    /// </summary>
    public void StartTransition() {
        inTransition = true;
        canMove = false;
    }

    /// <summary>
    /// Returns true if the player character is currently transitioning to a new scene.
    /// </summary>
    public bool IsInTransition() {
        return inTransition;
    }
}
