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
}

[Serializable]
public class SaveState {
    public InventoryData inventoryData = null;
    public List<ItemSaveState> itemData = new List<ItemSaveState>();
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
        UpdateSceneItems();
    }

    public void LoadSaveState() {
        if (File.Exists(SaveStateFile)) {
            string saveStateJSON = File.ReadAllText(SaveStateFile);
            currentSaveState = JsonUtility.FromJson<SaveState>(saveStateJSON);

            PlayerInventory.Instance?.SetInventoryData(currentSaveState.inventoryData);

            Debug.Log("Read SaveState from file.");
        } else {
            Debug.Log("SaveState file does not exist and can not be read.");
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

    public void UpdateItem(ItemSaveState itemSaveState) {
        int i = 0;
        for (i = 0; i < currentSaveState.itemData.Count; i++) {
            if (currentSaveState.itemData[i].name == itemSaveState.name) {
                break;
            }
        }
        if (i < currentSaveState.itemData.Count) { // found
            currentSaveState.itemData[i].collected = itemSaveState.collected;
        } else {
            currentSaveState.itemData.Add(itemSaveState);
        }
    }

    private void UpdateSceneItems() {
        Debug.Log("Updating item save states based on save file.");
        GameObject[] itemObjects = GameObject.FindGameObjectsWithTag("Item");
        foreach(GameObject itemObject in itemObjects) {
            Item item = itemObject.GetComponent<Item>();
            foreach(ItemSaveState state in currentSaveState.itemData) {
                if (item.uniqueIdentifier == state.name) {
                    item.UpdateSaveState(state);
                }
            }
        }
    }
}