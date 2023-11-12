using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    Camera cam;
    public Transform lookAtTarget;
    GameObject virtualCam;

    void Start() {
        cam = GetComponent<Camera>();
    }

    void Update() {
        if(virtualCam.GetComponent<CameraDataHolder>().camData.swivel && lookAtTarget != null) {
            cam.transform.LookAt(lookAtTarget);
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
