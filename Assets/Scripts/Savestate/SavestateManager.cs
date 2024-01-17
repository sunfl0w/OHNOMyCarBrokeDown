using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class PlayerSaveState {
    public bool initialized = false;
    public Vector3 position = Vector3.zero;
    public Quaternion rotation = Quaternion.identity;
    public string currentSceneName = String.Empty;
    public int health = 0;
    public float flashlightBatteryLife = 0.0f;
}

[Serializable]
public class SaveState {
    public InventoryData inventoryData = null;
    public List<InteractableSaveState> interactables = new List<InteractableSaveState>();
    public PlayerSaveState playerSaveState = new PlayerSaveState();
}

public class SavestateManager : MonoBehaviour {
    private static SavestateManager instance;
    public static SavestateManager Instance { get { return instance; } }

    private SaveState currentSaveState = new SaveState();

    private const string SaveStateFile = "SaveState.json";

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
        LoadSaveState();
    }

    private void Start() {
        UpdateSceneInteractables();
        PlayerInventory.Instance?.SetInventoryData(currentSaveState.inventoryData);
    }

    public void LoadSaveState() {
        if (File.Exists(SaveStateFile)) {
            string saveStateJSON = File.ReadAllText(SaveStateFile);
            currentSaveState = JsonUtility.FromJson<SaveState>(saveStateJSON);

            Debug.Log("Read SaveState from file.");
        } else {
            Debug.Log("SaveState file does not exist. An new one will be created.");
            string saveStateJSON = JsonUtility.ToJson(currentSaveState);
            File.WriteAllText(SaveStateFile, saveStateJSON);
            saveStateJSON = File.ReadAllText(SaveStateFile);
            currentSaveState = JsonUtility.FromJson<SaveState>(saveStateJSON);
        }
    }

    public void StoreSaveState() {
        currentSaveState.inventoryData = PlayerInventory.Instance.GetInventoryData();

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
        File.WriteAllText(SaveStateFile, saveStateJSON);
        Debug.Log("Store SaveState to file.");
    }

    public void ClearSaveState() {
        if (File.Exists(SaveStateFile)) {
            File.Delete(SaveStateFile);
            Debug.Log("Deleted SaveState file.");
        }
    }

    public SaveState GetCurrentSaveState() {
        return currentSaveState;
    }

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