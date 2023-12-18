using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SODEntityController : MonoBehaviour {
    public GameObject player;
    public float aggroRange = 10.0f;
    public float attackRange = 1.0f;
    public float rotationDampening = 3.0f;
    public float attackCooldownTime = 3.0f;

    private NavAgent navAgent;
    private Animator animator;

    private bool canAttack = true;

    void Awake() {
        navAgent = GetComponent<NavAgent>();
        navAgent.movementEnabled = false;
        animator = GetComponent<Animator>();
    }

    void Update() {
        if (player != null && Vector3.Distance(player.transform.position, this.transform.position) < aggroRange) {
            navAgent.movementEnabled = true;
        } else {
            navAgent.movementEnabled = false;
        }

        if (navAgent.movementEnabled) {
            Vector3 nextWaypoint = navAgent.GetNextWaypoint();
            if (Vector3.Distance(Vector3.positiveInfinity, nextWaypoint) > 1.0f) { // Only true if a next waypoint exists
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(nextWaypoint - transform.position, Vector3.up), Time.deltaTime * rotationDampening);
            }

            if (Vector3.Distance(player.transform.position, this.transform.position) < attackRange && canAttack) {
                animator.SetTrigger("AttackTrigger");
                canAttack = false;
                StartCoroutine(AttackCooldown());
            }
        }
    }

    IEnumerator AttackCooldown() {
        yield return new WaitForSeconds(attackCooldownTime);
        canAttack = true;
    }
}
