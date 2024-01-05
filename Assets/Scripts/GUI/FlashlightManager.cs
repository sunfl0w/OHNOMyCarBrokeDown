using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FlashlightManager : MonoBehaviour
{
    public float batteryLife = 100f; // Initial battery life in percentage
    public float depletionRate = 1f; // Rate at which battery depletes per second

    public TextMeshProUGUI batteryText;
    public GameObject batteryObject;

    public GameObject spotLight;

    private bool lightActive = false;

    private static FlashlightManager instance;

    public static FlashlightManager Instance { get { return instance; } }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void Update()
    {
        if (lightActive && batteryLife > 0)
        {
            batteryLife -= depletionRate * Time.deltaTime;
            batteryText.text = Mathf.Round(batteryLife).ToString();
        }
        if (batteryLife < 0 && lightActive)
        {
            spotLight.SetActive(false);
            lightActive = false;
        }
    }

    public void ChangeBattery()
    {
        spotLight.SetActive(true);
        batteryLife = 100f;
        lightActive = true;
    }

    public void EquipFlashlight()
    {
        batteryObject.SetActive(true);
        spotLight.SetActive(true);
        lightActive = true;
    }

    public void UnequipFlashlight()
    {
        batteryObject.SetActive(false);
        spotLight.SetActive(false);
        lightActive = false;

    }

}
