using UnityEngine;

public enum TerrainType { CONCRETE, GRASS, UNDEFINED };

public class TerrainChecker : MonoBehaviour {
    public Texture2D terrainTypeLookup;

    public TerrainType GetTerrainType() {
        LayerMask layerMask = LayerMask.GetMask("Terrain");
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, layerMask)) {
            // The lookup texture maps the texture coordinates of the terrain to the appropriate sound using the red color channel
            // This is a really simple and scalable and way of getting the terrain type
            Color color = terrainTypeLookup.GetPixel((int)(hit.textureCoord.x * terrainTypeLookup.width), (int)(hit.textureCoord.y * terrainTypeLookup.height));
            int terrainTypeIndex = Mathf.RoundToInt(color.r * 255.0f) / 16;
            //Debug.Log(color.r + " / " + hit.textureCoord.x + " / " + hit.textureCoord.y + " / " + terrainTypeIndex);
            if (terrainTypeIndex > 1) {
                return TerrainType.UNDEFINED;
            }
            return (TerrainType)terrainTypeIndex;
        }
        return TerrainType.UNDEFINED;
    }
}