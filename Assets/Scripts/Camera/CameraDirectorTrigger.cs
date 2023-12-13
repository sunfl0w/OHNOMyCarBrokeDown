using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDirectorTrigger : MonoBehaviour {
    public GameObject mainCamera;
    public GameObject virtualCamera;

    // When the player enters a camera trigger collider the connected "virtual camera" specifies the main cameras position and rotation.
    // Additionally the virtual camera provides a FOV value and enables or diables camera swivel
    private void OnTriggerEnter(Collider other) {
        mainCamera.GetComponent<CameraController>().setVirtualCamera(virtualCamera);
    }
}
