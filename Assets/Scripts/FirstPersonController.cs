using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour {
    public Camera playerCamera;
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float jumpPower = 4f;
    public float gravity = 10f;


    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    public Texture2D terrainTypeLookup;


    Vector3 moveVector = Vector3.zero;
    float rotationX = 0;

    public bool canMove = true;

    private bool isRunning = false;


    public enum TerrainType {GRASS, CONCRETE, UNDEFINED};

    CharacterController characterController;
    void Start() {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update() {

        #region Handles Movment
        // Press Left Shift to run
        isRunning = Input.GetKey(KeyCode.LeftShift);

        float movementDirectionY = moveVector.y;

        // Calculate normalized direction of movement based on input axis.
        // Normalization ensures that diagonal movement is as fast as movements along the x and z axis
        Vector3 moveDirection = ((Vector3.forward * Input.GetAxisRaw("Vertical")) + (Vector3.right * Input.GetAxisRaw("Horizontal"))).normalized;
        float speedModifier = canMove ? (isRunning && characterController.isGrounded ? runSpeed : walkSpeed) : 0;
        moveVector = moveDirection * speedModifier;

        #endregion

        #region Handles Jumping
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded) {
            moveVector.y = jumpPower;
        } else {
            moveVector.y = movementDirectionY;
        }

        if (!characterController.isGrounded) {
            moveVector.y -= gravity * Time.deltaTime;
        }

        #endregion

        #region Handles Rotation
        characterController.Move(transform.TransformDirection(moveVector) * Time.deltaTime);

        if (canMove) {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        #endregion
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
        LayerMask layerMask = LayerMask.GetMask("Terrain");
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, layerMask)) {
            // The lookup texture maps the texture coordinates of the terrain to the appropriate sound using the red color channel
            // This is a really simple and scalable and way of getting the terrain type
            Color color = terrainTypeLookup.GetPixel((int)(hit.textureCoord.x * terrainTypeLookup.width), (int)(hit.textureCoord.y * terrainTypeLookup.height));
            int terrainTypeIndex = Mathf.RoundToInt(color.r * 255.0f) / 32;
            //Debug.Log(color.r + " / " + hit.textureCoord.x + " / " + hit.textureCoord.y + " / " + terrainTypeIndex);
            if(terrainTypeIndex > 1) {
                return TerrainType.UNDEFINED;
            } 
            return (TerrainType)terrainTypeIndex;
        }
        return TerrainType.UNDEFINED;
    }
}