using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealthManager : MonoBehaviour
{
    public int health = 5;
    public TextMeshProUGUI playerHP;

    void Awake()
    {
        StalkingShadowEntityController.playerAttackedEvent += onPlayerAttacked;
    }

    void Update()
    {

    }

    void onPlayerAttacked(int damage)
    {
        Debug.Log("Player lost " + damage + " health due to an attack.");
        health -= damage;
        if (health >= 0)
        {
            playerHP.text = new string('♥', health);
        }
        else
        {
            Debug.Log("Game Over.");
        }


    }
}
