using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneGUIManager : MonoBehaviour
{
    [SerializeField] private HUDViewController hudViewController = default;
    [SerializeField] private PauseViewController pauseViewController = default;

    private bool gamepaused = default;

    private void Update()
    {
        CheckInputUpdate();
    }

    private void CheckInputUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gamepaused)
                HidePausePanel();
            else
                ShowPausePanel();
        }
    }

    private void ShowPausePanel()
    {
        hudViewController.gameObject.SetActive(false);
        pauseViewController.gameObject.SetActive(true);

        PauseGame();
    }

    public void HidePausePanel()
    {
        hudViewController.gameObject.SetActive(true);
        pauseViewController.gameObject.SetActive(false);

        ResumeGame();
    }

    public void HideAllPanels()
    {
        hudViewController.gameObject.SetActive(false);
        pauseViewController.gameObject.SetActive(false);

        ResumeGame();
    }

    private void PauseGame()
    {
        Time.timeScale = 0;

        gamepaused = true;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1;

        gamepaused = false;
    }
}
