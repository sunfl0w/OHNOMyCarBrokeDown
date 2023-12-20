using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour {
    public int health = 10;

    void Awake() {
        StalkingShadowEntityController.playerAttackedEvent += onPlayerAttacked;
    }

    void Update() {

    }

    void onPlayerAttacked(int damage) {
        Debug.Log("Player lost " + damage + " health due to an attack.");
        health -= damage;
    }
}
