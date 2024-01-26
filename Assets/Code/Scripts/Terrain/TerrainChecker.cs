using UnityEngine;

public enum TerrainType { CONCRETE, GRASS, GRAVEL, UNDEFINED };

/// <summary>
/// The terrain checker uses a lookup texture to find the current terrain type beneath the attached game object.
/// This works because the game uses a single texture atlas for all terrain models.
/// This is used to play the correct footstep sounds produced by the player character when moving.
/// </summary>
public class TerrainChecker : MonoBehaviour {
    /// <summary>
    /// Terrain lookup texture.
    /// </summary>
    public Texture2D terrainTypeLookup;

    /// <summary>
    /// Maximum valid terrain types.
    /// </summary>
    private static int MAX_VALID_TERRAIN_TYPES = 3;

    /// <summary>
    /// Maximum supported terrain types.
    /// </summary>
    private static int MAX_SUPPORTED_TERRAIN_TYPES = 16;

    /// <summary>
    /// Returns the terrain type beneath the attached game object based on the lookup texture.
    /// </summary>
    public TerrainType GetTerrainType() {
        LayerMask layerMask = LayerMask.GetMask("Terrain");
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, layerMask)) {
            // The lookup texture maps the texture coordinates of the terrain to the appropriate terrain type using the red color channel
            // This is a really simple and scalable way of getting the terrain type
            Color color = terrainTypeLookup.GetPixel((int)(hit.textureCoord.x * terrainTypeLookup.width), (int)(hit.textureCoord.y * terrainTypeLookup.height));
            int terrainTypeIndex = Mathf.RoundToInt(color.r * 255.0f) / MAX_SUPPORTED_TERRAIN_TYPES;
            if (terrainTypeIndex > MAX_VALID_TERRAIN_TYPES - 1) {
                return TerrainType.UNDEFINED;
            }
            return (TerrainType)terrainTypeIndex;
        }
        return TerrainType.UNDEFINED;
    }
}