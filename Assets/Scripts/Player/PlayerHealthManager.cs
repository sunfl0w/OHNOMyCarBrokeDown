using UnityEngine;

public class PlayerHealthManager : MonoBehaviour {
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

    public int GetPlayerHealth() {
        return health;
    }
}