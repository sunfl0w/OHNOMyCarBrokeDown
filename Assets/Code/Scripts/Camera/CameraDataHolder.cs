using UnityEngine;

/// <summary>
/// The camera data holder can be attached to a game object to make it hold virtual camera data.
/// This is useful as the game object's position is also the virtual camera's position.
/// Defining camera shots and angles is therefore much easier.
/// </summary>
public class CameraDataHolder : MonoBehaviour {
    public CameraData camData;
}
