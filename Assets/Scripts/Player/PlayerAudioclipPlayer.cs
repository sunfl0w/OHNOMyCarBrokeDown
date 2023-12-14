using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioclipPlayer : MonoBehaviour {
    public float maxVolume = 0.5f;
    public TerrainChecker playerTerrainChecker;
    public AudioSource playerAudioSource;

    public AudioClip[] stepGrassClips;
    public AudioClip[] stepConcreteClips;

    public void Awake() {
        playerTerrainChecker = GetComponent<TerrainChecker>();
        playerAudioSource = GetComponent<AudioSource>();
    }

    public void Footstep() {
        TerrainType terrainType = playerTerrainChecker.GetTerrainType();
        switch (terrainType) {
            case TerrainType.CONCRETE:
                playerAudioSource.pitch = Random.Range(0.9f, 1.1f);
                playerAudioSource.PlayOneShot(stepConcreteClips[Random.Range(0, stepConcreteClips.Length)], maxVolume);
                break;
            case TerrainType.GRASS:
                playerAudioSource.pitch = Random.Range(0.9f, 1.1f);
                playerAudioSource.PlayOneShot(stepGrassClips[Random.Range(0, stepGrassClips.Length)]);
                break;
            default:
                break;
        }
    }
}
