using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicDirector : MonoBehaviour {
    public AudioSource audioSource;
    public AudioClip currentMusicClip;

    void Start() {
        audioSource.clip = currentMusicClip;
        audioSource.Play();
    }
}
