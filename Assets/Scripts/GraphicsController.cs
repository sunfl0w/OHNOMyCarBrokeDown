using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GraphicsController : MonoBehaviour {
    public Color fogColor;
    public float fogCoefficient;
    public Material quadMaterial;
    public Material skyboxMaterial;
    public Material spotlightMaterial;

    void Start() {
    }

    void Update() {
        quadMaterial.SetColor("_Fog_Color", fogColor);
        quadMaterial.SetFloat("_Fog_Coefficient", fogCoefficient);
        skyboxMaterial.SetColor("_Fog_Color", fogColor);
        skyboxMaterial.SetFloat("_Fog_Coefficient", fogCoefficient);
        spotlightMaterial.SetColor("_Fog_Color", fogColor);
        spotlightMaterial.SetFloat("_Fog_Coefficient", fogCoefficient);
    }
}
