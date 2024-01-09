using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    Camera cam;
    public Transform lookAtTarget;
    public float targetYOffset = 1.0f;
    GameObject virtualCam;

    void Start() {
        cam = GetComponent<Camera>();
    }

    void Update() {
        if(virtualCam != null && virtualCam.GetComponent<CameraDataHolder>().camData.swivel && lookAtTarget != null) {
            cam.transform.LookAt(lookAtTarget.position + Vector3.up * targetYOffset);
        }
    }

    public void setVirtualCamera(GameObject virtualCam) {
        this.virtualCam = virtualCam;
        cam.fieldOfView = virtualCam.GetComponent<CameraDataHolder>().camData.fov;
        cam.transform.position = virtualCam.transform.position;
        cam.transform.rotation = virtualCam.transform.rotation;
    }

    public GameObject getCurrentVirtualCamera() {
        return virtualCam;
    }
}
