using System;
using UnityEngine;

/// <summary>
/// The player health manager stores the player character's health.
/// This behaviour is usually attached to the player character object.
/// </summary>
public class PlayerHealthManager : MonoBehaviour {
    /// <summary>
    /// Current player character health.
    /// </summary>
    private int health = 5;

    void Awake() {
        StalkingShadowEntityController.playerAttackedEvent += OnPlayerAttacked;
    }

    void Start() {
        if (SavestateManager.Instance.GetCurrentSaveState().playerSaveState.initialized) {
            int loadedHealth = SavestateManager.Instance.GetCurrentSaveState().playerSaveState.health;
            Debug.Log("Loaded player health of " + loadedHealth + " from current save state");
            health = loadedHealth;
        }
    }

    /// <summary>
    /// Called when the player was attacked.
    /// </summary>
    void OnPlayerAttacked(int damage) {
        Debug.Log("Player lost " + damage + " health due to an attack.");
        health -= damage;

    }

    /// <summary>
    /// Adds health to the player character.
    /// </summary>
    public void AddHealth(int health) {
        this.health = Math.Clamp(this.health + health, 0, 5);
    }

    /// <summary>
    /// Returns the player's current health.
    /// </summary>
    public int GetPlayerHealth() {
        return health;
    }
}