using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuViewController : MonoBehaviour
{
    [SerializeField] private InputField addressInput = default;
    [SerializeField] private InputField portInput = default;

    private void Start()
    {
        addressInput.text = NetworkConnectionManager.CurrentConnectionData.connectAddress;
        portInput.text = NetworkConnectionManager.CurrentConnectionData.connectPort.ToString();
    }

    public void CreateServerGameMLAPIPredictionMovementOnClick()
    {
        CreateServerGame(MovementPredictionTypes.MLAPIPredictionMovement);
    }

    public void CreateServerGameSinglePredictionMovementOnClick()
    {
        CreateServerGame(MovementPredictionTypes.SinglePredictionMovement);
    }

    public void CreateServerGameGroupedPredictionMovementOnClick()
    {
        CreateServerGame(MovementPredictionTypes.GroupedPredictionMovement);
    }

    private void CreateServerGame(MovementPredictionTypes movementPredictionType)
    {
        string connectAddress = addressInput.text;
        int connectPort = int.Parse(portInput.text);

        NetworkConnectionManager.CurrentConnectionData = new NetworkConnectionData(connectAddress, connectPort);

        GameFlowManager.StartServerGame(movementPredictionType);
    }

    public void CreateClientGameOnClick()
    {
        string connectAddress = addressInput.text;
        int connectPort = int.Parse(portInput.text);

        NetworkConnectionManager.CurrentConnectionData = new NetworkConnectionData(connectAddress, connectPort);

        GameFlowManager.StartClientGame();
    }

    public void QuitGameOnClick()
    {
        Application.Quit();
    }

    private GameFlowManager GameFlowManager => GameFlowManager.Instance;
    private NetworkConnectionManager NetworkConnectionManager => NetworkConnectionManager.Instance;
}
