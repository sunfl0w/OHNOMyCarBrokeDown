using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogDensityManager : MonoBehaviour {
    float baseFogDensity = 0.0f;
    float maxFogDensity = 1.0f;
    bool inInnerFog = true;
    void Start() {
        baseFogDensity = RenderSettings.fogDensity;
    }

    void Update() {
        LayerMask layerMask = LayerMask.GetMask("FogColliderInner", "FogColliderOuter");
        RaycastHit hit;

        int numInner = 0;
        int numOuter = 0;
        Physics.queriesHitBackfaces = true;
        for (int i = 0; i < 8; i++) {
            if (Physics.Raycast(transform.position, new Vector3(Mathf.Cos(i * 45.0f), 0.0f, Mathf.Sin(i * 45.0f)), out hit, 1.0f, layerMask)) {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("FogColliderInner")) {
                    numInner++;
                } else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("FogColliderOuter")) {
                    numOuter++;
                }
            }
        }
        Physics.queriesHitBackfaces = false;

        if (numInner > numOuter) {
            inInnerFog = true;
        } else if (numInner < numOuter) {
            inInnerFog = false;
        }

        if (inInnerFog) {
            RenderSettings.fogDensity = Mathf.Max(baseFogDensity, RenderSettings.fogDensity - 1.0f * Time.deltaTime);
        } else {
            RenderSettings.fogDensity = Mathf.Min(maxFogDensity, RenderSettings.fogDensity + 1.0f * Time.deltaTime);
        }
    }
}
