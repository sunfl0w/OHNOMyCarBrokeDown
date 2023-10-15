using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GraphicsController : MonoBehaviour {
    public Color fogColor;
    public float fogCoefficient;
    public Color ambientLightColor;
    public float ambientLightStrength;
    public Material quadMaterial;
    public Material skyboxMaterial;
    public Material spotlightMaterial;

    void Start() {
    }

    void Update() {
        // Maybe only set a value if it changed?
        quadMaterial.SetColor("_Fog_Color", fogColor);
        quadMaterial.SetFloat("_Fog_Coefficient", fogCoefficient);
        quadMaterial.SetColor("_Ambient_Light_Color", ambientLightColor);
        quadMaterial.SetFloat("_Ambient_Light_Strength", ambientLightStrength);

        skyboxMaterial.SetColor("_Fog_Color", fogColor);
        skyboxMaterial.SetFloat("_Fog_Coefficient", fogCoefficient);

        spotlightMaterial.SetColor("_Fog_Color", fogColor);
        spotlightMaterial.SetFloat("_Fog_Coefficient", fogCoefficient);
    }
}
