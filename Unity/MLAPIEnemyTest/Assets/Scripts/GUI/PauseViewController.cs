using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseViewController : MonoBehaviour
{
    public void ResumeGameOnClick()
    {
        
    }

    public void QuitGameOnClick()
    {
        GameFlowManager.DisconnectAndLoadMainMenu();

        HidePausePanel();
    }

    private void HidePausePanel()
    {
        gameObject.SetActive(false);
    }

    private GameFlowManager GameFlowManager => GameFlowManager.Instance;
}
