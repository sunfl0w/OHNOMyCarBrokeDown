using UnityEngine;

public class PlayerInput : MonoBehaviour {
    private Vector3 moveDirection = Vector3.zero;
    private bool movementInputEnabled = true;

    void Awake() {
        UnifiedGUI.GUIEnterEvent += OnGUIEnter;
        UnifiedGUI.GUILeaveEvent += EnableMovementInput;
    }

    void Start() {
        // Lock cursor and make it invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnDestroy() {
        UnifiedGUI.GUIEnterEvent -= OnGUIEnter;
        UnifiedGUI.GUILeaveEvent -= EnableMovementInput;
    }

    void FixedUpdate() {
        moveDirection = Vector3.zero;
        if (movementInputEnabled) {
            // Input of vertical axis is clamped to never be below 0 so that the player can't move bachwards
            moveDirection = ((transform.forward * Mathf.Max(Input.GetAxisRaw("Vertical"), 0.0f)) + (transform.right * Input.GetAxisRaw("Horizontal"))).normalized;
        }
    }

    public Vector3 GetMoveDirection() {
        return moveDirection;
    }

    public void EnableMovementInput() {
        movementInputEnabled = true;
    }

    public void DisableMovementInput() {
        movementInputEnabled = false;
    }

    public bool IsMovementInputEnabled() {
        return movementInputEnabled;
    }

    public void OnGUIEnter(bool enableGUIBlur, bool disableMovement) {
        if (disableMovement) { // Disable player movement if desired
            DisableMovementInput();
        }
    }
}