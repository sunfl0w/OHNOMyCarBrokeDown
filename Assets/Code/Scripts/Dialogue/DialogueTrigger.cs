using UnityEngine;

/// <summary>
/// The dialogue trigger holds dialogue data that is presented to the player via the dialogue gui when the attached collider was entered.
/// </summary>
public class DialogueTrigger : MonoBehaviour {
    /// <summary>
    /// Attached dialogue data.
    /// </summary>
    public DialogueData data;

    /// <summary>
    /// Stores trigger state to prevent repeated triggering.
    /// </summary>
    private bool firstTrigger = true;

    private void OnTriggerEnter(Collider other) {
        if(firstTrigger) {
            Debug.Log("Opening dialogue gui");
            DialogueGUI.Instance.Show(data);
            firstTrigger = false;
        }
    }
}
