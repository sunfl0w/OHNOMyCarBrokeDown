using UnityEngine;

/// <summary>
/// The player footsteps clip player plays footstep sound clips when moving.
/// </summary>
public class PlayerFootstepsPlayer : MonoBehaviour {
    /// <summary>
    /// Max volume of audio clips.
    /// </summary>
    public float maxVolume = 0.5f;

    /// <summary>
    /// Reference to the terrain checker.
    /// </summary>
    public TerrainChecker playerTerrainChecker;

    /// <summary>
    /// Reference to the player character audio source.
    /// </summary>
    public AudioSource playerAudioSource;

    /// <summary>
    /// Array of grass footstep clips.
    /// </summary>
    public AudioClip[] stepGrassClips;

    /// <summary>
    /// Array of concrete footstep clips.
    /// </summary>
    public AudioClip[] stepConcreteClips;

    /// <summary>
    /// Array of gravel footstep clips.
    /// </summary>
    public AudioClip[] stepGravelClips;

    public void Awake() {
        playerTerrainChecker = GetComponent<TerrainChecker>();
        playerAudioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Called by the player animator when the player character's foot is put on the ground while walking.
    /// </summary>
    public void Footstep() {
        TerrainType terrainType = playerTerrainChecker.GetTerrainType();
        switch (terrainType) {
            case TerrainType.CONCRETE:
                playerAudioSource.pitch = Random.Range(0.9f, 1.1f);
                playerAudioSource.PlayOneShot(stepConcreteClips[Random.Range(0, stepConcreteClips.Length)], maxVolume);
                break;
            case TerrainType.GRASS:
                playerAudioSource.pitch = Random.Range(0.9f, 1.1f);
                playerAudioSource.PlayOneShot(stepGrassClips[Random.Range(0, stepGrassClips.Length)], maxVolume * 1.5f);
                break;
            case TerrainType.GRAVEL:
                playerAudioSource.pitch = Random.Range(0.9f, 1.1f);
                playerAudioSource.PlayOneShot(stepGravelClips[Random.Range(0, stepGravelClips.Length)], maxVolume);
                break;
            default:
                break;
        }
    }
}
