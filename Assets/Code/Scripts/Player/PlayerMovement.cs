using UnityEngine;

/// <summary>
/// The third person player controller controls the player character based on user input.
/// </summary>
public class PlayerMovement : MonoBehaviour {
    public float walkSpeed = 2.2f;
    public float gravityAcceleration = 10f;
    public float translationDampening = 3.0f;
    public float rotationDampening = 5.0f;

    private Vector3 targetMoveDirection = Vector3.zero;
    private float currentTranslationSpeed = 0.0f;
    private bool inTransition = false;

    private PlayerInput playerInput;
    private CharacterController characterController;
    private Animator animator;

    void Awake() {
        UnifiedGUI.GUIEnterEvent += OnGUIEnter;
    }

    void OnDestroy() {
        UnifiedGUI.GUIEnterEvent -= OnGUIEnter;
    }

    void Start() {
        playerInput = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate() {
        // "Tank-Control" player movement
        targetMoveDirection = playerInput.GetMoveDirection();
        float targetSpeed = 0.0f;
        if (playerInput.IsMovementInputEnabled() && targetMoveDirection.magnitude > 0.0f) {
            targetSpeed = walkSpeed;
        }
        currentTranslationSpeed = Mathf.Lerp(currentTranslationSpeed, targetSpeed, Time.deltaTime * translationDampening);
        if (targetMoveDirection.magnitude > 0.0f && playerInput.IsMovementInputEnabled()) { // Only rotate if the player inputs a movement direction
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetMoveDirection, Vector3.up), Time.deltaTime * rotationDampening);
        }
        animator.SetFloat("movementSpeed", currentTranslationSpeed); // Update animator based on current speed so the animator can choose the correct animation state

        if (playerInput.IsMovementInputEnabled()) { // Only move player if movement input is allowed
            characterController.Move((transform.forward * currentTranslationSpeed + Vector3.down * gravityAcceleration) * Time.deltaTime);
        }
    }

    public bool GetIsMoving() {
        return characterController.attachedRigidbody.velocity.magnitude > 0.1f;
    }

    public bool GetIsGrounded() {
        return characterController.isGrounded;
    }

    public void OnGUIEnter(bool enableGUIBlur, bool disableMovement) {
        if (disableMovement) { // Disable player movement if desired
            currentTranslationSpeed = 0.0f;
        }
    }

    public void StartTransition() {
        inTransition = true;
    }

    public bool IsInTransition() {
        return inTransition;
    }
}
