using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseViewController : MonoBehaviour
{
    [SerializeField] private GameSceneGUIManager gameSceneGUIManager = default;

    public void ResumeGameOnClick()
    {
        gameSceneGUIManager.HidePausePanel();
    }

    public void QuitGameOnClick()
    {
        GameFlowManager.DisconnectAndLoadMainMenu();

        gameSceneGUIManager.HideAllPanels();
    }

    private GameFlowManager GameFlowManager => GameFlowManager.Instance;
}
