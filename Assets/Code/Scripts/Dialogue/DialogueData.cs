using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "ScriptableObjects/DialogueData", order = 1)]
public class DialogueData : ScriptableObject {
    [TextArea]
    public string text;

    public DialogueData(string text) {
        this.text = text;
    }
}