using UnityEngine;

[CreateAssetMenu(fileName = "InteractableData", menuName = "ScriptableObjects/InteractableData", order = 1)]
public class InteractableData : ScriptableObject {
    public string interactText;

    public InteractableData(string interactText) {
        this.interactText = interactText;
    }
}
