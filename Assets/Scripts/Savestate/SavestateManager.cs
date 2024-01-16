using System;
using System.IO;
using UnityEngine;

[Serializable]
public class SaveState {
    public InventoryData inventoryData;
}

public class SavestateManager : MonoBehaviour {
    private static SavestateManager instance;
    public static SavestateManager Instance { get { return instance; } }

    private const string SaveStateFile = "SaveState.json";

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
    }

    private void Start() {
        LoadSaveState();
    }

    public void LoadSaveState() {
        if (File.Exists(SaveStateFile)) {
            string saveStateJSON = File.ReadAllText(SaveStateFile);
            SaveState saveState = JsonUtility.FromJson<SaveState>(saveStateJSON);
            PlayerInventory.Instance.SetInventoryData(saveState.inventoryData);
            Debug.Log("Read SaveState from file.");
        } else {
            Debug.Log("SaveState file does not exist and can not be read.");
        }
    }

    public void StoreSaveState() {
        SaveState newSaveState = new SaveState();
        newSaveState.inventoryData = PlayerInventory.Instance.GetInventoryData();

        string saveStateJSON = JsonUtility.ToJson(newSaveState);
        File.WriteAllText(SaveStateFile, saveStateJSON);
        Debug.Log("Store SaveState to file.");
    }

    public void ClearSaveState() {
        if (File.Exists(SaveStateFile)) {
            File.Delete(SaveStateFile);
            Debug.Log("Deleted SaveState file.");
        }
    }
}