using UnityEngine;

/// <summary>
/// The camera controller moves and rotates the current camera based on player position.
/// A new camera position is set when the player enters a camera director trigger, which in turn notifies the camera controller.
/// </summary>
public class CameraController : MonoBehaviour {
    /// <summary>
    /// The current target the camera should look at.
    /// </summary>
    public Transform lookAtTarget;

    /// <summary>
    /// The camera target's y offset. This is used to account for the targets position relative to the terrain.
    /// </summary>
    public float targetYOffset = 1.0f;

    /// <summary>
    /// Camera smooth time used to smooth camera translation.
    /// </summary>
    private static float CAM_TRANSLATION_SMOOTH_TIME = 0.1f;

    /// <summary>
    /// Camera dampening coefficient used to smooth camera rotation.
    /// </summary>
    private static float CAM_ROTATION_DAMPENING_COEFFICIENT = 3.0f;

    /// <summary>
    /// The current camera.
    /// </summary>
    private Camera cam;

    /// <summary>
    /// Reference to the unified gui.
    /// </summary>
    private UnifiedGUI unifiedGUI;

    /// <summary>
    /// Reference to the current virtual camera game object. The currently used camera data is attached to this object.
    /// </summary>
    private GameObject virtualCam;

    /// <summary>
    /// Current camera velocity.
    /// </summary>
    private Vector3 camVelocity = Vector3.zero;

    void Start() {
        unifiedGUI = GameObject.FindGameObjectWithTag("UnifiedGUI").GetComponent<UnifiedGUI>();
        cam = GetComponent<Camera>();
    }

    void Update() {
        if (virtualCam == null && lookAtTarget != null) { // Prevent the case in which no virtual camera is set
            Debug.Log("No virtual camera set. Trying to find closest trigger and its repsective virtual camera");
            Collider[] colliderArray = Physics.OverlapSphere(lookAtTarget.position, 40.0f, LayerMask.GetMask("CamTrigger"));
            GameObject closestTrigger = null;
            float closestDistance = 999.0f;
            foreach (Collider collider in colliderArray) {
                float dst = Vector3.Distance(collider.transform.position, lookAtTarget.position);
                if (dst < closestDistance) {
                    closestDistance = dst;
                    closestTrigger = collider.gameObject;
                }
            }

            if (closestTrigger != null) {
                SetVirtualCamera(closestTrigger.GetComponent<CameraDirectorTrigger>().virtualCamera);
            }
        }

        CameraData camData = virtualCam?.GetComponent<CameraDataHolder>().camData;
        if (virtualCam != null && (DialogueGUI.Instance.IsVisible() || !unifiedGUI.IsAnyGUIVisible())) { // Prevent camera movement when any gui is visible
            if (camData.camType == CamType.FixedSwivel && lookAtTarget != null) { // Fixed swivel cam
                cam.transform.LookAt(lookAtTarget.position + Vector3.up * targetYOffset);
            } else if ((camData.camType == CamType.Follow || camData.camType == CamType.FollowSwivel) && lookAtTarget != null) { // Follow cam
                // Camera follows look at target based on position and desired distance between the two.
                // Movement is damped to be less irritating.
                Vector3 camPosTarget = GetCameraPositionTarget(camData);
                Vector3 camToTarget = lookAtTarget.position - cam.transform.position;
                cam.transform.position = Vector3.SmoothDamp(cam.transform.position, camPosTarget, ref camVelocity, CAM_TRANSLATION_SMOOTH_TIME);
                if (camData.camType == CamType.FollowSwivel) {
                    cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, Quaternion.LookRotation(camToTarget, Vector3.up), Time.deltaTime * CAM_ROTATION_DAMPENING_COEFFICIENT);
                }
            }
        }
    }

    /// <summary>
    /// Sets the current virtual camera.
    /// This is used by camer director triggers.
    /// </summary>
    public void SetVirtualCamera(GameObject virtualCam) {
        if (virtualCam != this.virtualCam) { // Only set a new virtual camera when it is different compared to the old virtual camera.
            this.virtualCam = virtualCam;
            CameraData camData = virtualCam?.GetComponent<CameraDataHolder>().camData;
            cam.fieldOfView = camData.fov;
            cam.transform.position = virtualCam.transform.position;
            cam.transform.rotation = virtualCam.transform.rotation;

            if ((camData.camType == CamType.Follow || camData.camType == CamType.FollowSwivel) && lookAtTarget != null) {
                // Instantly move follow cam to target position before letting it follow
                cam.transform.position = GetCameraPositionTarget(camData);
                Vector3 camToTarget = lookAtTarget.position - cam.transform.position;
                cam.transform.rotation = Quaternion.LookRotation(camToTarget, Vector3.up);
            }
        }
    }

    /// <summary>
    /// Returns the current virtual camera.
    /// </summary>
    public GameObject GetCurrentVirtualCamera() {
        return virtualCam;
    }

    /// <summary>
    /// Computes the camera target position for follow camera types.
    /// It tries to follow the target while moving along a specified axis, while keeping a fixed distance to the target.
    /// </summary>
    Vector3 GetCameraPositionTarget(CameraData camData) {
        Vector3 camToTarget = lookAtTarget.position - cam.transform.position;
        Vector3 projectedFollowAxis = Vector3.Project(camToTarget - camData.followAxis.normalized * camData.followDistance, camData.followAxis);
        return cam.transform.position + projectedFollowAxis;
    }
}
