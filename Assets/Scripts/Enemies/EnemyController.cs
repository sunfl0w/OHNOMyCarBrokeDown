using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {
    public NavMeshAgent navmeshAgent;
    public GameObject target;
    public float maxTargetDistance = 10.0f;

    void Start() {

    }

    void Update() {
        if(Vector3.Distance(navmeshAgent.transform.position, target.transform.position) <= maxTargetDistance) {
            navmeshAgent.destination = target.transform.position;
            navmeshAgent.isStopped = false;
        } else {
            navmeshAgent.isStopped = true;
        }
    }
}
