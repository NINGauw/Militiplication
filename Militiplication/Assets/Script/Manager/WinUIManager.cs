using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class WinUIManager : MonoBehaviour
{
    public static WinUIManager Instance { get; private set;}
    public GameObject winPanel;
    public float delayBeforeShow = 1.5f;
    public Button buttonBackMenu;
    public Button nextStage;
    public Button restartStage;
    private bool hasTriggered = false;
    void Start()
    {
        Instance = this;
        if (winPanel != null)
            winPanel.SetActive(false);

        //Button
        buttonBackMenu.onClick.AddListener(BackMenu);
        nextStage.onClick.AddListener(NextStage);
        restartStage.onClick.AddListener(Restart);  
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void NextStage()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void BackMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void TriggerWin()
    {
        if (hasTriggered) return;
        hasTriggered = true;
        Invoke(nameof(ShowWinPanel), delayBeforeShow);
    }

    private void ShowWinPanel()
    {
        if (winPanel != null)
            winPanel.SetActive(true);
        Debug.Log("🎉 WinPanel Shown!");
    }
}