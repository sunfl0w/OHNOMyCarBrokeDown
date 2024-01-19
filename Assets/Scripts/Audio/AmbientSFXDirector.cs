using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSFXDirector : MonoBehaviour {
    public List<AudioClip> ambientClips;

    private AudioSource audioSource;

    void Start() {
        audioSource = GetComponent<AudioSource>();
        if (ambientClips.Count > 0) {
            StartCoroutine(PlayClip());
        }
    }

    private IEnumerator PlayClip() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(10.0f, 30.0f));
            audioSource.pitch = Random.Range(0.93f, 1.07f);
            audioSource.PlayOneShot(ambientClips[Random.Range(0, ambientClips.Count)]);
        }
    }
}
