using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Serializable player save state data used to store and load player related information.
/// </summary>
[Serializable]
public class PlayerSaveState {
    public bool initialized = false;
    public Vector3 position = Vector3.zero;
    public Quaternion rotation = Quaternion.identity;
    public string currentSceneName = String.Empty;
    public int health = 0;
    public float flashlightBatteryLife = 0.0f;
}

/// <summary>
/// Serializable save state containing player data, inventory data and interactable data.
/// </summary>
[Serializable]
public class SaveState {
    public InventorySaveState inventorySaveState = null;
    public List<InteractableSaveState> interactables = new List<InteractableSaveState>();
    public PlayerSaveState playerSaveState = new PlayerSaveState();
}

/// <summary>
/// The save state manager provides save and store functionality to the game.
/// It is used to make game progress persistent between scenes and applications starts by saving data in a file on disk.
/// Note that the save state manager is a singleton.
/// </summary>
public class SavestateManager : MonoBehaviour {
    private static SavestateManager instance;
    public static SavestateManager Instance { get { return instance; } }

    /// <summary>
    /// The current save state.
    /// </summary>
    private SaveState currentSaveState = new SaveState();

    /// <summary>
    /// File name of the save state on disk.
    /// </summary>
    private const string SAVE_STATE_FILE = "SaveState.json";

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
        LoadSaveState(); // Load save state from disk on awake
    }

    private void Start() {
        UpdateSceneInteractables();
        PlayerInventory.Instance?.SetInventorySaveState(currentSaveState.inventorySaveState);
    }

    /// <summary>
    /// Load the save state from disk.
    /// </summary>
    public void LoadSaveState() {
        if (File.Exists(SAVE_STATE_FILE)) {
            string saveStateJSON = File.ReadAllText(SAVE_STATE_FILE);
            currentSaveState = JsonUtility.FromJson<SaveState>(saveStateJSON);

            Debug.Log("Read SaveState from file.");
        } else {
            Debug.Log("SaveState file does not exist. An new one will be created.");
            string saveStateJSON = JsonUtility.ToJson(currentSaveState);
            File.WriteAllText(SAVE_STATE_FILE, saveStateJSON);
            saveStateJSON = File.ReadAllText(SAVE_STATE_FILE);
            currentSaveState = JsonUtility.FromJson<SaveState>(saveStateJSON);
        }
    }

    /// <summary>
    /// Store save state to disk.
    /// </summary>
    public void StoreSaveState() {
        currentSaveState.inventorySaveState = PlayerInventory.Instance.GetInventorySaveState();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) {
            currentSaveState.playerSaveState.initialized = true;
            currentSaveState.playerSaveState.position = player.transform.position;
            currentSaveState.playerSaveState.rotation = player.transform.rotation;
            currentSaveState.playerSaveState.currentSceneName = SceneManager.GetActiveScene().name;
            currentSaveState.playerSaveState.health = player.GetComponent<PlayerHealthManager>().GetPlayerHealth();
            currentSaveState.playerSaveState.flashlightBatteryLife = player.GetComponent<PlayerFlashlightManager>().GetBatteryLife();
        }

        string saveStateJSON = JsonUtility.ToJson(currentSaveState);
        File.WriteAllText(SAVE_STATE_FILE, saveStateJSON);
        Debug.Log("Store SaveState to file.");
    }

    /// <summary>
    /// Clear save state on disk.
    /// </summary>
    public void ClearSaveState() {
        if (File.Exists(SAVE_STATE_FILE)) {
            File.Delete(SAVE_STATE_FILE);
            Debug.Log("Deleted SaveState file.");
        }
    }

    /// <summary>
    /// Returns the current save state.
    /// </summary>
    public SaveState GetCurrentSaveState() {
        return currentSaveState;
    }

    // TODO
    public void UpdateInteractable(InteractableSaveState saveState) {
        int i = 0;
        for (i = 0; i < currentSaveState.interactables.Count; i++) {
            if (currentSaveState.interactables[i].name == saveState.name) {
                break;
            }
        }
        if (i < currentSaveState.interactables.Count) { // found
            currentSaveState.interactables[i].interacted = saveState.interacted;
        } else {
            currentSaveState.interactables.Add(saveState);
        }
    }

    /// <summary>
    /// Update all interactables in the currently loaded scene base on save state data.
    /// </summary>
    private void UpdateSceneInteractables() {
        Debug.Log("Updating item save states based on save file.");
        GameObject[] itemObjects = GameObject.FindGameObjectsWithTag("Item");
        foreach(GameObject itemObject in itemObjects) {
            foreach(InteractableSaveState state in currentSaveState.interactables) {
                if (itemObject.GetComponent<Item>().GetUniqueIdentifier() == state.name) {
                    itemObject.GetComponent<Item>().UpdateSaveState(state);
                }
            }
        }

        Debug.Log("Updating other interactable save states based on save file.");
        GameObject[] interactableObjects = GameObject.FindGameObjectsWithTag("Interactable");
        foreach(GameObject interactableObject in interactableObjects) {
            foreach(InteractableSaveState state in currentSaveState.interactables) {
                if (interactableObject.GetComponent<IInteractable>().GetUniqueIdentifier() == state.name) {
                    interactableObject.GetComponent<IInteractable>().UpdateSaveState(state);
                }
            }
        }
    }
}