using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gameover : MonoBehaviour
{

    private UnifiedGUI unifiedGUI;

    public static event Action<bool, bool> GameoverEnterEvent;
    public static event Action GameoverLeaveEvent;

    private static Gameover instance;
    public static Gameover Instance { get { return instance; } }

    private void Awake()
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

    // Start is called before the first frame update
    void Start()
    {
        unifiedGUI = GameObject.FindGameObjectWithTag("UnifiedGUI").GetComponent<UnifiedGUI>();
        Hide();
    }

    void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void Show()
    {
        GameoverEnterEvent?.Invoke(false, true);
        this.gameObject.SetActive(true);

        StartCoroutine(BackToMainMenu());

    }

    private IEnumerator BackToMainMenu()
    {
        yield return new WaitForSeconds(3f);
        GameoverLeaveEvent?.Invoke();
        SavestateManager.Instance.ClearSaveState();
        SceneTransitionManager.Instance.LoadScene("MainMenuScene");
    }


}
