using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The health indicator gui displays the healthbar of the player character.
/// The health indicator gui is a singleton.
/// </summary>
public class HealthIndicatorGUI : MonoBehaviour {
    /// <summary>
    /// Healthbar heart image array.
    /// </summary>
    private Image[] healthImages;

    /// <summary>
    /// Reference to the player health manager.
    /// </summary>
    private PlayerHealthManager playerHealthManager;

    private static HealthIndicatorGUI instance;
    public static HealthIndicatorGUI Instance { get { return instance; } }

    void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
    }

    void Start() {
        healthImages = GetComponentsInChildren<Image>();
        playerHealthManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthManager>();
    }

    void Update() {
        int playerHealth = playerHealthManager.GetPlayerHealth();
        for (int i = 0; i < healthImages.Length; i++) {
            healthImages[i].enabled = playerHealth > i;
        }
        if (playerHealth <= 0) {
            GameoverGUI.Instance.Show();
        }
    }

    /// <summary>
    /// Hide health indicator gui.
    /// </summary>
    public void Hide() {
        gameObject.SetActive(false);
    }
}