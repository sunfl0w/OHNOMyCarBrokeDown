using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CameraData", menuName = "ScriptableObjects/CameraData", order = 1)]
public class CameraData : ScriptableObject {
    public float fov;
    public bool swivel;
}