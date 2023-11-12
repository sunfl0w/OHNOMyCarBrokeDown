using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioDirector : MonoBehaviour {
    public float maxVolume = 1.0f;
    public ThirdPersonPlayerController thirdPersonController;
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
        if(thirdPersonController.GetIsMoving() && thirdPersonController.GetIsGrounded()) {
            playerAudioSource.volume = maxVolume;
            ThirdPersonPlayerController.TerrainType terrainType = thirdPersonController.GetTerrainType();
            if(terrainType == ThirdPersonPlayerController.TerrainType.GRASS) {
                if(thirdPersonController.GetIsRunning() && playerAudioSource.clip != runGrassClip) {
                    playerAudioSource.clip = runGrassClip;
                    playerAudioSource.Play();
                } else if (!thirdPersonController.GetIsRunning() && playerAudioSource.clip != walkGrassClip) {
                    playerAudioSource.clip = walkGrassClip;
                    playerAudioSource.Play();
                }
            } else if(terrainType == ThirdPersonPlayerController.TerrainType.CONCRETE) {
                if(thirdPersonController.GetIsRunning() && playerAudioSource.clip != runConcreteClip) {
                    playerAudioSource.clip = runConcreteClip;
                    playerAudioSource.Play();
                } else if (!thirdPersonController.GetIsRunning() && playerAudioSource.clip != walkConcreteClip) {
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
