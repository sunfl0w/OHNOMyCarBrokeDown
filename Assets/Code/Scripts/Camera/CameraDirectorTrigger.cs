using UnityEngine;

/// <summary>
/// The camera director trigger sets the current virtual camera used by the camera controller when its collider was entered
/// </summary>
public class CameraDirectorTrigger : MonoBehaviour {
    /// <summary>
    /// Reference to the main camera.
    /// </summary>
    public GameObject mainCamera;

    /// <summary>
    /// Reference to the linked virtual camera.
    /// </summary>
    public GameObject virtualCamera;

    private void OnTriggerEnter(Collider other) {
        // When the player enters a camera trigger collider the connected virtual camera specifies the main cameras position and rotation.
        mainCamera.GetComponent<CameraController>().SetVirtualCamera(virtualCamera);
    }
}
