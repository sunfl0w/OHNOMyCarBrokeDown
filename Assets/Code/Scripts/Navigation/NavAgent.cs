using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sadly Unity does not support custom navmeshes so I programmed a workable replacement.
/// The NavAgent component is used in conjunction with a CharacterController component to move the attached GameObject.
/// </summary>
public class NavAgent : MonoBehaviour {
    /// <summary>
    /// Navmesh used for pathfinding by the agent.
    /// </summary>
    public NavMesh navmesh;

    /// <summary>
    /// Character controller of the agent.
    /// </summary>
    public CharacterController characterController;

    /// <summary>
    /// Target transform of the agent. This is the position the agent tries to move toward.
    /// </summary>
    public Transform targetTransform;

    /// <summary>
    /// Agent ground offset. Useful when the agents position is above the actual navmesh.
    /// </summary>
    public float agentGroundOffset = 0.0f;

    /// <summary>
    /// Specifies if the agent is able to move.
    /// </summary>
    public bool movementEnabled = true;

    /// <summary>
    /// Current path.
    /// </summary>
    private List<Vector3> path;

    /// <summary>
    /// Current path index used to define the current node in the current path.
    /// </summary>
    private int currentPathIndex = 0;

    /// <summary>
    /// The maximum distance between agent and path node such that the node is registered as reached.
    /// </summary>
    private static float MAX_PATH_NODE_DISTANCE = 0.3f;

    /// <summary>
    /// Path update interval in seconds.
    /// </summary>
    private static float PATH_UPDATE_INTERVAL = 2.0f;

    void Start() {
        StartCoroutine(UpdatePath());
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        if (path != null && currentPathIndex < path.Count) {
            Gizmos.DrawSphere(path[currentPathIndex], 0.5f);
        }

    }

    void FixedUpdate() {
        if (path != null && currentPathIndex < path.Count && movementEnabled) { // Follow path if able and path is valid
            Vector3 currPos = this.transform.position;
            currPos.y -= agentGroundOffset;
            if (Vector3.Distance(path[currentPathIndex], currPos) < MAX_PATH_NODE_DISTANCE) { // Close enough to current path node, got to next one
                currentPathIndex++;
            }

            if (currentPathIndex < path.Count) {
                // Move along path
                Vector3 motion = new Vector3(path[currentPathIndex].x - this.transform.position.x, 0, path[currentPathIndex].z - this.transform.position.z);
                characterController.Move(motion.normalized * Time.deltaTime);
            }
        }
    }

    /// <summary>
    /// Returns the next waypoint of the current path.
    /// </summary>
    public Vector3 GetNextWaypoint() {
        if (path != null && currentPathIndex < path.Count) {
            return path[currentPathIndex];
        }
        return Vector3.positiveInfinity;
    }

    /// <summary>
    /// Coroutine to update the current path asynchronosly based on PATH_UPDATE_INTERVAL.
    /// This is used to follow a moving target.
    /// </summary>
    IEnumerator UpdatePath() {
        while (true) {
            if (movementEnabled) {
                Vector3 startPos = this.transform.position;
                startPos.y -= agentGroundOffset;
                path = navmesh.GetPath(startPos, targetTransform.position);
                currentPathIndex = 0;
            }
            yield return new WaitForSeconds(Mathf.Max(0.1f, PATH_UPDATE_INTERVAL));
        }
    }
}
