using System.Collections;
using UnityEngine;
using System;

public class StalkingShadowEntityController : MonoBehaviour {
    public GameObject player;
    public float aggroRange = 10.0f;
    public float attackRange = 1.0f;
    public float rotationDampening = 3.0f;
    public float attackCooldownTime = 8.0f;
    public float flashlightCooldownTime = 10.0f;
    public int attackDamage = 1;

    public static event Action<int> playerAttackedEvent;

    private NavAgent navAgent;
    private Animator animator;
    private AudioSource audioSource;

    private bool canAttack = true;
    private bool canMove = true;

    void Awake() {
        navAgent = GetComponent<NavAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update() {
        navAgent.movementEnabled = canMove;

        if (canMove) {
            Vector3 nextWaypoint = navAgent.GetNextWaypoint();
            if (Vector3.Distance(Vector3.positiveInfinity, nextWaypoint) > 1.0f) { // Only true if a next waypoint exists
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(nextWaypoint - transform.position, Vector3.up), Time.deltaTime * rotationDampening);
            }
        }

        // This only triggers the attack animation and cooldown coroutine
        // The test if the attack hit the player is perforemd in the Attack method called while the animation is running as a trigger
        if (Vector3.Distance(player.transform.position, this.transform.position) < attackRange && canAttack) {
            animator.SetTrigger("AttackTrigger");
            canAttack = false;
            StartCoroutine(AttackCooldown());
        }

        // Checks if the entity is hit by the player flashlight
        RaycastHit hit;
        Debug.DrawRay(player.transform.position + Vector3.up, player.transform.forward, Color.red);
        bool flashlightActive = player.GetComponent<PlayerFlashlightManager>().IsFlashlightActive();
        if (canMove && flashlightActive && Physics.Raycast(player.transform.position + Vector3.up, player.transform.forward, out hit, 10.0f, LayerMask.GetMask("Entity"))) {
            if (hit.transform.gameObject.GetInstanceID() == this.gameObject.GetInstanceID()) {
                canAttack = false;
                canMove = false;
                GetComponent<CharacterController>().excludeLayers |= (1 << LayerMask.NameToLayer("Player"));
                StartCoroutine(FlashlightCooldown());
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

    void AttackSound() {
        audioSource.Play();
    }

    IEnumerator AttackCooldown() {
        yield return new WaitForSeconds(attackCooldownTime);
        canAttack = true;
    }

    IEnumerator FlashlightCooldown() {
        yield return new WaitForSeconds(flashlightCooldownTime);
        canAttack = true;
        canMove = true;
        GetComponent<CharacterController>().excludeLayers &= ~(1 << LayerMask.NameToLayer("Player"));
    }
}
