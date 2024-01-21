using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HealthIndicatorGUI : MonoBehaviour
{
    private Image indicatorImage;
    private bool isVisible = false;
    private bool flashlightActive = true;

    private Image[] healthImages;
    private PlayerHealthManager playerHealthManager;

    //public GameObject spotLight;
    //public float originalIntensity = 50f;
    //private bool isFlickering = false;

    //private bool lightActive = false;

    private static HealthIndicatorGUI instance;

    public static HealthIndicatorGUI Instance { get { return instance; } }

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

    void Start()
    {
        healthImages = GetComponentsInChildren<Image>();
        playerHealthManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthManager>();
    }

    void Update()
    {
        int playerHealth = playerHealthManager.GetPlayerHealth();
        for (int i = 0; i < healthImages.Length; i++)
        {
            healthImages[i].enabled = playerHealth > i;
        }
        if (playerHealth <= 0)
        {
            Gameover.Instance.Show();
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    /*void Update() {
        if (lightActive && batteryLife > 0) {
            batteryLife -= depletionRate * Time.deltaTime;
            batteryText.text = Mathf.Round(batteryLife).ToString();
            if (batteryLife < 10 && !isFlickering) {
                isFlickering = true;
                StartCoroutine(Flicker());
            }
        }
        if (batteryLife < 0 && lightActive) {
            spotLight.SetActive(false);
            lightActive = false;
        }
    }

    private IEnumerator Flicker() {

        while (batteryLife < 10) {
            // Flicker effect
            spotLight.GetComponent<Light>().intensity = Random.Range(originalIntensity * 0.5f, originalIntensity * 0.8f);

            yield return new WaitForSeconds(0.1f); // Adjust the duration between flickers
        }
        isFlickering = false;
    }
    public void ChangeBattery() {
        spotLight.SetActive(true);
        batteryLife = 100f;
        lightActive = true;
        spotLight.GetComponent<Light>().intensity = originalIntensity;
        isFlickering = false;
    }

    public void EquipFlashlight() {
        batteryObject.SetActive(true);
        spotLight.SetActive(true);
        lightActive = true;
    }

    public void UnequipFlashlight() {
        batteryObject.SetActive(false);
        spotLight.SetActive(false);
        lightActive = false;

    }*/
}