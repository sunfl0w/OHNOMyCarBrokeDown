using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicDirector : MonoBehaviour {
    public List<AudioClip> tracks = new List<AudioClip>();

    private AudioSource audioSource;

    void Start() {
        audioSource = GetComponent<AudioSource>();
        if (tracks.Count > 0) {
            StartCoroutine(PlayTrack());
        }
    }

    private IEnumerator PlayTrack() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(10.0f, 60.0f));
            audioSource.PlayOneShot(tracks[Random.Range(0, tracks.Count)]);
        }
    }
}
