using System.Collections;
using UnityEngine;
using System;

/// <summary>
/// The stalking shadow entity controller controls the entity based on player movement, position and flashlight use.
/// The entity tries to follow and kill the player.
/// </summary>
public class StalkingShadowEntityController : MonoBehaviour {
    /// <summary>
    /// Reference to the player game object.
    /// </summary>
    public GameObject player;

    /// <summary>
    /// Range in which the entity starts to follow the player.
    /// </summary>
    public float aggroRange = 10.0f;

    /// <summary>
    /// Max distance between entity and player to trigger an attack by the entity.
    /// </summary>
    public float attackRange = 1.0f;

    /// <summary>
    /// Rotation dampening coefficient used in entity movement.
    /// </summary>
    public float rotationDampening = 3.0f;

    /// <summary>
    /// Minimum time between entity attacks.
    /// </summary>
    public float attackCooldownTime = 8.0f;

    /// <summary>
    /// Minimum time between the entity being stunned by the flashlight and it starting to move and attack again.
    /// </summary>
    public float flashlightCooldownTime = 10.0f;

    /// <summary>
    /// Damage dealt to player when the entity attacks,
    /// </summary>
    public int attackDamage = 1;

    /// <summary>
    /// Action invoked when the entity attacks the player.
    /// </summary>
    public static event Action<int> playerAttackedEvent;

    /// <summary>
    /// Nav agent reference.
    /// </summary>
    private NavAgent navAgent;

    /// <summary>
    /// Animator reference.
    /// </summary>
    private Animator animator;

    /// <summary>
    /// Audio source reference.
    /// </summary>
    private AudioSource audioSource;

    /// <summary>
    /// Flag specifying, whether the entity can attack the player currently.
    /// </summary>
    private bool canAttack = true;

    /// <summary>
    /// Flag specifying, whether the entity can move currently.
    /// </summary>
    private bool canMove = true;

    void Awake() {
        navAgent = GetComponent<NavAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update() {
        navAgent.movementEnabled = canMove;
        if (player.GetComponent<ThirdPersonPlayerController>().IsInTransition()) {
            canAttack = false;
            canMove = false;
        }

        if (canMove) { // Rotate entity according to the next waypoint
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

                // If the entity is stunned the player can move through it
                GetComponent<CharacterController>().excludeLayers |= 1 << LayerMask.NameToLayer("Player");

                StartCoroutine(FlashlightCooldown());
            }
        }
    }

    /// <summary>
    /// Is called by the attack animation at the time of attack.
    /// </summary>
    void Attack() {
        if (Physics.Raycast(this.transform.position + Vector3.up, this.transform.forward, attackRange + 10.0f, LayerMask.GetMask("Player"))) {
            playerAttackedEvent?.Invoke(attackDamage);
        }
    }

    /// <summary>
    /// Is called by the attack animation at the time of attack.
    /// </summary>
    void AttackSound() {
        audioSource.Play();
    }

    /// <summary>
    /// Coroutine acting as an attack cooldown timer.
    /// </summary>
    IEnumerator AttackCooldown() {
        yield return new WaitForSeconds(attackCooldownTime);
        canAttack = true;
    }

    /// <summary>
    /// Coroutine acting as a flashlight stun cooldown timer.
    /// </summary>
    IEnumerator FlashlightCooldown() {
        yield return new WaitForSeconds(flashlightCooldownTime);
        canAttack = true;
        canMove = true;

        // If the entity is no longer stunned the player can't move through it
        GetComponent<CharacterController>().excludeLayers &= ~(1 << LayerMask.NameToLayer("Player"));
    }
}
