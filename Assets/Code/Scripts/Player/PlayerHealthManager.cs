using System;
using UnityEngine;

/// <summary>
/// The player health manager stores the player character's health.
/// This behaviour is usually attached to the player character object.
/// </summary>
public class PlayerHealthManager : MonoBehaviour {
    public int maxHealth = 5;
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

    void OnPlayerAttacked(int damage) {
        Debug.Log("Player lost " + damage + " health due to an attack.");
        health -= damage;

    }

    public void AddHealth(int health) {
        this.health = Math.Clamp(this.health + health, 0, maxHealth);
    }

    public bool CanHeal() {
        return health < maxHealth;
    }

    public int GetPlayerHealth() {
        return health;
    }
}