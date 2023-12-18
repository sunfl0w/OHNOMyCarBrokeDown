using UnityEngine;

[CreateAssetMenu(fileName = "InteractableData", menuName = "ScriptableObjects/InteractableData", order = 1)]
public class InteractableData : ScriptableObject {
    public string interactText;
    public AudioClip interactSound;

    public InteractableData(string interactText, AudioClip interactSound) {
        this.interactText = interactText;
        this.interactSound = interactSound;
    }
}
