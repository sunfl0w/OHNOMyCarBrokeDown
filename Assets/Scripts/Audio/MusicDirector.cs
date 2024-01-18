using UnityEngine;
using System.Collections.Generic;

public class MusicDirector : MonoBehaviour {
    public List<AudioClip> tracks = new List<AudioClip>();

    private AudioSource audioSource;

    void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    void Update() {
        if(!audioSource.isPlaying && tracks.Count > 0) {
            audioSource.PlayOneShot(tracks[Random.Range(0, tracks.Count)]);
        }
    }
}
