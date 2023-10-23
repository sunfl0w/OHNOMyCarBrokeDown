using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioDirector : MonoBehaviour {
    public float maxVolume = 1.0f;
    public FirstPersonController firstPersonController;
    public AudioSource playerAudioSource;

    public AudioClip walkGrassClip;
    public AudioClip runGrassClip;
    public AudioClip walkConcreteClip;
    public AudioClip runConcreteClip;

    void Start() {
        playerAudioSource.clip = walkGrassClip;
    }

    void Update() {
        // TODO: Code needs to be refactored
        // Based on the player controller and the terrain type play appropriate walking/running sound clips
        if(firstPersonController.GetIsMoving() && firstPersonController.GetIsGrounded()) {
            playerAudioSource.volume = maxVolume;
            FirstPersonController.TerrainType terrainType = firstPersonController.GetTerrainType();
            if(terrainType == FirstPersonController.TerrainType.GRASS) {
                if(firstPersonController.GetIsRunning() && playerAudioSource.clip != runGrassClip) {
                    playerAudioSource.clip = runGrassClip;
                    playerAudioSource.Play();
                } else if (!firstPersonController.GetIsRunning() && playerAudioSource.clip != walkGrassClip) {
                    playerAudioSource.clip = walkGrassClip;
                    playerAudioSource.Play();
                }
            } else if(terrainType == FirstPersonController.TerrainType.CONCRETE) {
                if(firstPersonController.GetIsRunning() && playerAudioSource.clip != runConcreteClip) {
                    playerAudioSource.clip = runConcreteClip;
                    playerAudioSource.Play();
                } else if (!firstPersonController.GetIsRunning() && playerAudioSource.clip != walkConcreteClip) {
                    playerAudioSource.clip = walkConcreteClip;
                    playerAudioSource.Play();
                }
            } else {
                playerAudioSource.Stop();
                playerAudioSource.time = 0.0f;
            }
        } else {
            playerAudioSource.time = 0.0f;
            playerAudioSource.volume = 0.0f;
        }
    }
}
