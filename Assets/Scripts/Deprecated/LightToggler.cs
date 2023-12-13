using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightToggler : MonoBehaviour {
    // Unity is really bad at deciding which lights are enabled and which are disabled.
    // This script tries to toggle a light based on distance to the main camera to remedy this issue

    public Transform mainCameraTransform;
    public GameObject lightObject;
    public float toggleDistance = 20.0f;
    void Update() {
        if(Vector3.Distance(mainCameraTransform.position, this.transform.position) > toggleDistance) {
            lightObject.SetActive(false);
        } else {
            lightObject.SetActive(true);
        }
    }
}
