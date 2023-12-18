using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanceInteractable : MonoBehaviour, IInteractable {
    public string targetSceneName;
    public string targetTransformName;
    public InteractableData data;
    public AudioSource audioSource;

    public void Interact() {
        audioSource.PlayOneShot(data.interactSound);
        Debug.Log("Interactable. Switch to scene: " + targetSceneName);
        SceneTransitionManager.Instance.SetNextTransformByName(targetTransformName);
        StartCoroutine(Transition());
    }

    public Transform GetTransform() {
        return transform;
    }

    public InteractableData GetData() {
        return data;
    }

    IEnumerator Transition() {
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene(targetSceneName, LoadSceneMode.Single);
    }
}
