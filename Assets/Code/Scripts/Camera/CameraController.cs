using UnityEngine;

public class CameraController : MonoBehaviour {
    Camera cam;
    public Transform lookAtTarget;
    public float targetYOffset = 1.0f;
    public float camTranslationSmoothTime = 0.1f;
    public float camRotationDampeningCoefficient = 3.0f;

    private UnifiedGUI unifiedGUI;
    private GameObject virtualCam;
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
        if (virtualCam != null && (DialogueGUI.Instance.IsVisible() || !unifiedGUI.IsAnyGUIVisible())) {
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

        /*var windowAspect = (float)Screen.width / Screen.height;
        var d = windowAspect / (4.0f / 3.0f);

        if (d > 1.0f) {
            Rect rect = cam.rect;
            rect.width = 1.0f / d;
            rect.height = 1.0f;
            rect.x = (1.0f - rect.width) / 2.0f;
            rect.y = 0;
            cam.rect = rect;
        } else {
            Rect rect = cam.rect;
            rect.height = 1.0f / d;
            rect.width = 1.0f;
            rect.y = (1.0f - rect.height) / 2.0f;
            rect.x = 0;
            cam.rect = rect;
        }*/
    }

    public void SetVirtualCamera(GameObject virtualCam) {
        if (virtualCam != this.virtualCam) {
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

    public GameObject GetCurrentVirtualCamera() {
        return virtualCam;
    }

    Vector3 GetCameraPositionTarget(CameraData camData) {
        Vector3 camToTarget = lookAtTarget.position - cam.transform.position;
        Vector3 projectedFollowAxis = Vector3.Project(camToTarget - camData.followAxis.normalized * camData.followDistance, camData.followAxis);
        return cam.transform.position + projectedFollowAxis;
    }
}
