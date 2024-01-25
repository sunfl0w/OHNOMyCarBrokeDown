using UnityEngine;

public enum CamType {
    Fixed,
    FixedSwivel,
    Follow,
    FollowSwivel,
}

[CreateAssetMenu(fileName = "CameraData", menuName = "ScriptableObjects/CameraData", order = 1)]
public class CameraData : ScriptableObject {
    public CamType camType;
    public float fov;
    public bool swivel;
    public float followDistance;
    public Vector3 followAxis;
}