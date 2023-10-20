using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour {
    public float movementSpeed = 4.0f;
    public float mouseSensitivity = 250.0f;
    private float groundOffset = 0.85f;
    private float minWallDistance = 1.0f;

    void Update() {
        float verticalTranslation = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;
        float horizontalTranslation = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;

        transform.Translate(horizontalTranslation, 0, verticalTranslation);

        float rotationY = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        transform.localEulerAngles = new Vector3(0.0f, rotationY, 0.0f);
    }

    // Just for testing purposes
    void OnGUI() {
        if (GUI.Button(new Rect(0, 0, 100, 50), "Lock Cursor")) {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
