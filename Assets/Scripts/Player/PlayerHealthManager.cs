using UnityEngine;

public class PlayerHealthManager : MonoBehaviour {
    [SerializeField]
    private int health = 5;

    void Awake() {
        StalkingShadowEntityController.playerAttackedEvent += OnPlayerAttacked;
    }

    void OnPlayerAttacked(int damage) {
        Debug.Log("Player lost " + damage + " health due to an attack.");
        health -= damage;
    }

    public int GetPlayerHealth() {
        return health;
    }
}