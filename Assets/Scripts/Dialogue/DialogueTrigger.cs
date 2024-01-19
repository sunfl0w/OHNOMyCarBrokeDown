using UnityEngine;

public class DialogueTrigger : MonoBehaviour {
    public DialogueData data;
    bool firstTrigger = true;
    private UnifiedGUI unifiedGUI;

    private void Start() {
        unifiedGUI = GameObject.FindGameObjectWithTag("UnifiedGUI").GetComponent<UnifiedGUI>();
    }

    private void OnTriggerEnter(Collider other) {
        if(firstTrigger) {
            Debug.Log("Opening dialogue gui");
            DialogueGUI.Instance.Show(data);
            firstTrigger = false;
        }
    }
}
