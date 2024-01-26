using UnityEngine;

/// <summary>
/// Simple data object to hold dialogue data. A scriptable object is actually not really required for this use case.
/// </summary>
[CreateAssetMenu(fileName = "DialogueData", menuName = "ScriptableObjects/DialogueData", order = 1)]
public class DialogueData : ScriptableObject {
    [TextArea]
    public string text;

    public DialogueData(string text) {
        this.text = text;
    }
}