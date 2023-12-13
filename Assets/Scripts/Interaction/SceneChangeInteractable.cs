using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanceInteractable : MonoBehaviour, IInteractable {
    public string targetSceneName;
    public string targetTransformName;
    public InteractableData data;

    public void Interact() {
        Debug.Log("Interactable. Switch to scene: " + targetSceneName);
        SceneTransitionManager.Instance.SetNextTransformByName(targetTransformName);
        SceneManager.LoadScene(targetSceneName, LoadSceneMode.Single);
    }

    public Transform GetTransform() {
        return transform;
    }

    public InteractableData GetData() {
        return data;
    }
}
