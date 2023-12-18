using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSFXDirector : MonoBehaviour {
    public AudioSource audioSource;
    public List<AudioClip> ambientClips;
    public float rollCooldown = 10.0f;
    public float sfxRollChance = 0.3f;

    private bool roll = true;

    void Update() {
        if(roll) {
            roll = false;
            if (Random.Range(0.0f, 1.0f) <= sfxRollChance) { // Play some ambient sound
                audioSource.PlayOneShot(ambientClips[Random.Range(0, ambientClips.Count)]); // Chose ambient sound randomly from list of sounds
            }
        }
    }

    IEnumerator RollCooldown() {
        yield return new WaitForSeconds(rollCooldown);
        roll = true;
    }
}
