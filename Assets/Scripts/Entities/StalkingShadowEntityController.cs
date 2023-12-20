using System.Collections;
using UnityEngine;
using System;

public class StalkingShadowEntityController : MonoBehaviour {
    public GameObject player;
    public float aggroRange = 10.0f;
    public float attackRange = 1.0f;
    public float rotationDampening = 3.0f;
    public float attackCooldownTime = 8.0f;
    public int attackDamage = 5;

    public static event Action<int> playerAttackedEvent;

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

            // This only triggers the attack animation and cooldown coroutine.
            // The test if the attack hit the player is perforemd in the Attack method called while the animation is running as a trigger
            if (Vector3.Distance(player.transform.position, this.transform.position) < attackRange && canAttack) {
                animator.SetTrigger("AttackTrigger");
                canAttack = false;
                StartCoroutine(AttackCooldown());
            }
        }
    }

    /// <summary>
    /// Is called by the attack animation at the time of attack
    /// </summary>
    void Attack() {
        if (Physics.Raycast(this.transform.position + Vector3.up, this.transform.forward, attackRange + 10.0f, LayerMask.GetMask("Player"))) {
            playerAttackedEvent?.Invoke(attackDamage);
        }
    }

    IEnumerator AttackCooldown() {
        yield return new WaitForSeconds(attackCooldownTime);
        canAttack = true;
    }
}
