using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerHealthManager : MonoBehaviour
{
    public int health = 5;
    public TextMeshProUGUI playerHP;

    public GameObject gameOverCanvas;
    public GameObject gui;

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
        if (health > 0)
        {
            playerHP.text = new string('♥', health);
        }
        else
        {
            gameOverCanvas.SetActive(true);
            gui.SetActive(false);
            Invoke("loadMainMenu", 3f);
        }


    }

    void loadMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
