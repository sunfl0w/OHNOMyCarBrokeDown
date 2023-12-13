using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour {
    private static SceneTransitionManager instance;
    public static SceneTransitionManager Instance { get { return instance; } }

    private string targetTransformName;

    private void Awake() {
        DontDestroyOnLoad(gameObject);
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += SetPlayerTransform;
    }
    
    private void OnDisable() {
        SceneManager.sceneLoaded -= SetPlayerTransform;
    }

    public void SetNextTransformByName(string targetTransformName) {
        this.targetTransformName = targetTransformName;
    }

    private void SetPlayerTransform(Scene scene, LoadSceneMode mode) {
        if(targetTransformName == null) {
            Debug.Log("No spawn point for player found as it is null. This might be intendet.");
        } else {
            GameObject spawnPoint = GameObject.Find(targetTransformName);
            if(spawnPoint == null) {
                Debug.Log("No spawn point for player found with name " + targetTransformName + ". Not changing player position.");
            } else {
                GameObject player = GameObject.FindWithTag("Player");
                Debug.Log("PP: " + player.transform.position);
                player.GetComponent<CharacterController>().enabled = false; // Need to disable controller to change transform directly
                player.transform.position = spawnPoint.transform.position;
                player.transform.rotation = spawnPoint.transform.rotation;
                player.GetComponent<CharacterController>().enabled = true;
                Debug.Log("Set player position and rotation based on target spawn point: " + spawnPoint.transform.position);
                Debug.Log("PP: " + player.transform.position);
            }
        }
    }
}
