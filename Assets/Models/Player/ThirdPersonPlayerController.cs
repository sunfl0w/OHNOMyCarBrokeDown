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

    public Texture2D terrainTypeLookup;

    Vector3 moveVector = Vector3.zero;
    Vector3 targetMoveDirection = Vector3.zero;
    public bool canMove = true;
    private bool isRunning = false;
    public float currentSpeed = 0.0f;
    public float translationDampening = 3.0f;
    public float rotationDampening = 5.0f;

    public enum TerrainType { GRASS, CONCRETE, UNDEFINED };

    CharacterController characterController;
    CameraController camController;

    void Start() {
        characterController = GetComponent<CharacterController>();
        camController = mainCamera.GetComponent<CameraController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void FixedUpdate() {
        // Press Left Shift to run
        //isRunning = Input.GetKey(KeyCode.LeftShift);

        // Player movement based on camera position
        /*GameObject virtualCam = camController.getCurrentVirtualCamera();
        Vector3 camForward = Vector3.zero;
        Vector3 camRight = Vector3.zero;
        if(virtualCam != null) {
            camForward = virtualCam.transform.forward;
            camRight = virtualCam.transform.right;
        }
        camForward.y = 0;
        camForward.Normalize();
        camRight.y = 0;
        camRight.Normalize();
        // Normalization ensures that diagonal movement is as fast as movements along the x and z axis
        targetMoveDirection = ((camForward * Input.GetAxisRaw("Vertical")) + (camRight * Input.GetAxisRaw("Horizontal"))).normalized;
        float targetSpeed = canMove && targetMoveDirection.magnitude > 0.0f ? (isRunning && characterController.isGrounded ? runSpeed : walkSpeed) : 0;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * translationDampening);
        if (targetMoveDirection.magnitude > 0.0f) { // Only rotate if the player inputs a movement direction
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetMoveDirection, Vector3.up), Time.deltaTime * rotationDampening);
        }
        animator.SetFloat("movementSpeed", currentSpeed);

        moveVector = transform.forward * currentSpeed;
        characterController.Move((moveVector + Vector3.down * gravity) * Time.deltaTime);*/

        // "Tank-Control" player movement
        targetMoveDirection = ((transform.forward * Input.GetAxisRaw("Vertical")) + (transform.right * Input.GetAxisRaw("Horizontal"))).normalized;
        float targetSpeed = canMove && targetMoveDirection.magnitude > 0.0f ? (isRunning && characterController.isGrounded ? runSpeed : walkSpeed) : 0;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * translationDampening);
        if (targetMoveDirection.magnitude > 0.0f) { // Only rotate if the player inputs a movement direction
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

    public TerrainType GetTerrainType() {
        LayerMask layerMask = LayerMask.GetMask("TerrainCollider");
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, layerMask)) {
            // The lookup texture maps the texture coordinates of the terrain to the appropriate sound using the red color channel
            // This is a really simple and scalable and way of getting the terrain type
            Color color = terrainTypeLookup.GetPixel((int)(hit.textureCoord.x * terrainTypeLookup.width), (int)(hit.textureCoord.y * terrainTypeLookup.height));
            int terrainTypeIndex = Mathf.RoundToInt(color.r * 255.0f) / 32;
            //Debug.Log(color.r + " / " + hit.textureCoord.x + " / " + hit.textureCoord.y + " / " + terrainTypeIndex);
            if (terrainTypeIndex > 1) {
                return TerrainType.UNDEFINED;
            }
            return (TerrainType)terrainTypeIndex;
        }
        return TerrainType.UNDEFINED;
    }
}
