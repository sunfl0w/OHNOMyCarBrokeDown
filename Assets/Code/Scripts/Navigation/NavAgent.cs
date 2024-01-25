using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sadly Unity does not support custom navmeshes so I programmed a workable replacement.
/// The NavAgent component is used in conjunction with a CharacterController component to move the attached GameObject.
/// 
/// </summary>
public class NavAgent : MonoBehaviour {
    public NavMesh navmesh;
    public CharacterController characterController;
    public Transform targetTransform;
    public float agentGroundOffset = 0.0f;
    public float pathUpdateInterval = 2.0f;
    public bool movementEnabled = true;

    private List<Vector3> path;
    private int currentPathIndex = 0;

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
        if (path != null && currentPathIndex < path.Count && movementEnabled) {
            Vector3 currPos = this.transform.position;
            currPos.y -= agentGroundOffset;
            if (Vector3.Distance(path[currentPathIndex], currPos) < 0.3f) {
                currentPathIndex++;
            }

            if (currentPathIndex < path.Count) {
                Vector3 motion = new Vector3(path[currentPathIndex].x - this.transform.position.x, 0, path[currentPathIndex].z - this.transform.position.z);
                characterController.Move(motion.normalized * Time.deltaTime);
            }
        }
    }

    public Vector3 GetNextWaypoint() {
        if(path != null && currentPathIndex < path.Count) {
            return path[currentPathIndex];
        }
        return Vector3.positiveInfinity;
    }

    IEnumerator UpdatePath() {
        while (true) {
            if (movementEnabled) {
                Vector3 startPos = this.transform.position;
                startPos.y -= agentGroundOffset;
                path = navmesh.GetPath(startPos, targetTransform.position);
                currentPathIndex = 0;
            }
            yield return new WaitForSeconds(Mathf.Max(0.1f, pathUpdateInterval));
        }
    }
}
