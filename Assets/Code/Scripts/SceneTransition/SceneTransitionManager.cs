using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour {
    private static SceneTransitionManager instance;
    public static SceneTransitionManager Instance { get { return instance; } }

    private string targetTransformName = String.Empty;
    private string lastActiveSceneName = String.Empty;

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

    public void LoadScene(string sceneName) {
        Debug.Log("Loading scene named " + sceneName);
        lastActiveSceneName = SceneManager.GetActiveScene().name;
        targetTransformName = String.Empty;
        SceneManager.LoadScene(sceneName);
    }

    public void LoadSceneWithNextPlayerTransform(string sceneName, string targetTransformName) {
        Debug.Log("Loading scene named " + sceneName + " with target player tranform " + targetTransformName);
        lastActiveSceneName = SceneManager.GetActiveScene().name;
        this.targetTransformName = targetTransformName;
        SceneManager.LoadScene(sceneName);
    }

    private void SetPlayerTransform(Scene scene, LoadSceneMode mode) {
        if(targetTransformName == null || targetTransformName == String.Empty) {
            if (lastActiveSceneName == "MainMenuScene" && SavestateManager.Instance.GetCurrentSaveState().playerSaveState.initialized) {
                // We come from the main menu so position the player at the last known position
                GameObject player = GameObject.FindWithTag("Player");
                player.GetComponent<CharacterController>().enabled = false; // Need to disable controller to change transform directly
                player.transform.position = SavestateManager.Instance.GetCurrentSaveState().playerSaveState.position;
                player.transform.rotation = SavestateManager.Instance.GetCurrentSaveState().playerSaveState.rotation;
                player.GetComponent<CharacterController>().enabled = true;
                Debug.Log("Set player position based on current save state.");
            } else {
                Debug.Log("No spawn point for player found as it is null.");
            }
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
