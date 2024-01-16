using UnityEngine;

public class CameraController : MonoBehaviour {
    Camera cam;
    public Transform lookAtTarget;
    public float targetYOffset = 1.0f;
    public float camTranslationSmoothTime = 0.1f;
    public float camRotationDampeningCoefficient = 3.0f;

    private GameObject virtualCam;
    private Vector3 camVelocity = Vector3.zero;

    void Start() {
        cam = GetComponent<Camera>();
    }

    void Update() {
        CameraData camData = virtualCam?.GetComponent<CameraDataHolder>().camData;
        if (virtualCam != null) {
            if (camData.camType == CamType.FixedSwivel && lookAtTarget != null) {
                cam.transform.LookAt(lookAtTarget.position + Vector3.up * targetYOffset);
            } else if ((camData.camType == CamType.Follow || camData.camType == CamType.FollowSwivel) && lookAtTarget != null) {
                Vector3 camPosTarget = GetCameraPositionTarget(camData);
                Vector3 camToTarget = lookAtTarget.position - cam.transform.position;
                cam.transform.position = Vector3.SmoothDamp(cam.transform.position, camPosTarget, ref camVelocity, camTranslationSmoothTime);
                if (camData.camType == CamType.FollowSwivel) {
                    cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, Quaternion.LookRotation(camToTarget, Vector3.up), Time.deltaTime * camRotationDampeningCoefficient);
                }
            }
        }
    }

    public void SetVirtualCamera(GameObject virtualCam) {
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

    public GameObject GetCurrentVirtualCamera() {
        return virtualCam;
    }

    Vector3 GetCameraPositionTarget(CameraData camData) {
        Vector3 camToTarget = lookAtTarget.position - cam.transform.position;
        Vector3 projectedFollowAxis = Vector3.Project(camToTarget - camData.followAxis.normalized * camData.followDistance, camData.followAxis);
        return cam.transform.position + projectedFollowAxis;
    }
}
