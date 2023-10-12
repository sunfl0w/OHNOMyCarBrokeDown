using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCameraController : MonoBehaviour {
    public float mouse_sensitivity = 5.0f;
    public float translation_speed = 10.0f;
    private bool view_mode = false;
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (view_mode) {
            float rot_y = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mouse_sensitivity;
            float rot_x = transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * mouse_sensitivity;
            transform.localEulerAngles = new Vector3(rot_x, rot_y, 0f);
        }

        float current_translation_speed = translation_speed;
        if (Input.GetKey(KeyCode.LeftShift)) {
            current_translation_speed *= 3.0f;
        }

        if (Input.GetKey(KeyCode.W)) {
            transform.position = transform.position + (transform.forward * current_translation_speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.S)) {
            transform.position = transform.position + (-transform.forward * current_translation_speed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            view_mode = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        } else if (Input.GetKeyUp(KeyCode.Mouse1)) {
            view_mode = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
