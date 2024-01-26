using UnityEngine;
using System;
using UnityEngine.SceneManagement;

/// <summary>
/// The scene transition manager is used to transition form one scene to another.
/// This manager handles the player position based on save state data and other transition infromation provided by scene transition interactables.
/// The manager is persitent between scenes as it is not destroyed on load.
/// </summary>
public class SceneTransitionManager : MonoBehaviour {
    private static SceneTransitionManager instance;
    public static SceneTransitionManager Instance { get { return instance; } }

    private string playerTargetTransformName = String.Empty;
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

    /// <summary>
    /// Load a scene by name.
    /// </summary>
    public void LoadScene(string sceneName) {
        Debug.Log("Loading scene named " + sceneName);
        lastActiveSceneName = SceneManager.GetActiveScene().name;
        playerTargetTransformName = String.Empty;
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Load a scene based on name.
    /// Also sets the target transform name.
    /// This is used by scene change interactables to provide spawn poinst to the player character.
    /// </summary>
    public void LoadSceneWithNextPlayerTransform(string sceneName, string targetTransformName) {
        Debug.Log("Loading scene named " + sceneName + " with target player tranform " + targetTransformName);
        lastActiveSceneName = SceneManager.GetActiveScene().name;
        this.playerTargetTransformName = targetTransformName;
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Sets the player transform based on the last scene, target player transform and save state data.
    /// This is usually called after a new scene was loaded.
    /// </summary>
    private void SetPlayerTransform(Scene scene, LoadSceneMode mode) {
        if(playerTargetTransformName == null || playerTargetTransformName == String.Empty) {
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
            GameObject spawnPoint = GameObject.Find(playerTargetTransformName);
            if(spawnPoint == null) {
                Debug.Log("No spawn point for player found with name " + playerTargetTransformName + ". Not changing player position.");
            } else {
                GameObject player = GameObject.FindWithTag("Player");
                player.GetComponent<CharacterController>().enabled = false; // Need to disable controller to change transform directly
                player.transform.position = spawnPoint.transform.position;
                player.transform.rotation = spawnPoint.transform.rotation;
                player.GetComponent<CharacterController>().enabled = true;
                Debug.Log("Set player position and rotation based on target spawn point: " + spawnPoint.transform.position);
            }
        }
    }
}
