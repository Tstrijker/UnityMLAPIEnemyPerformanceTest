using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuViewController : MonoBehaviour
{
    [SerializeField] private InputField addressInput = default;
    [SerializeField] private InputField portInput = default;
    [SerializeField] private GUIOptionPicker enemyMovePicker = default;
    [SerializeField] private GUIOptionPicker enemyPredicionPicker = default;
    [SerializeField] private GUIOptionPicker enemyDataSendingPicker = default;
    [SerializeField] private InputField sendRatePerSecondInput = default;
    [SerializeField] private InputField bufferWaitTimeInput = default;
    [SerializeField] private InputField spawnNumberOfEnemiesInput = default;
    [SerializeField] private Toggle simulateDropingPackagesToggle = default;
    [SerializeField] private InputField dropEachSetNumberPackageInput = default;

    private void Start()
    {
        addressInput.text = NetworkConnectionManager.CurrentConnectionData.connectAddress;
        portInput.text = NetworkConnectionManager.CurrentConnectionData.connectPort.ToString();

        enemyMovePicker.Setup(typeof(EnemyMoveTypes), (int)GameSettingsData.enemyMoveType);
        enemyPredicionPicker.Setup(typeof(MovementPredictionTypes), (int)GameSettingsData.predictionType);
        enemyDataSendingPicker.Setup(typeof(MovementPredictionDataTypes), (int)GameSettingsData.movementPredictionData);

        sendRatePerSecondInput.text = GameSettingsData.sendRatePerSecond.ToString();
        bufferWaitTimeInput.text = GameSettingsData.bufferWaitTime.ToString();
        spawnNumberOfEnemiesInput.text = GameSettingsData.spawnNumberOfEnemies.ToString();
        simulateDropingPackagesToggle.isOn = GameSettingsData.simulateDropingPackages;
        dropEachSetNumberPackageInput.text = GameSettingsData.dropEachSetNumberPackage.ToString();
    }

    public void CreateServerGame()
    {
        SetSettings();

        GameFlowManager.StartServerGame();
    }

    public void CreateClientGameOnClick()
    {
        SetSettings();

        GameFlowManager.StartClientGame();
    }

    public void QuitGameOnClick()
    {
        Application.Quit();
    }

    private void SetSettings()
    {
        string connectAddress = addressInput.text;
        int connectPort = int.Parse(portInput.text);

        NetworkConnectionManager.CurrentConnectionData = new NetworkConnectionData(connectAddress, connectPort);

        GameSettingsData.enemyMoveType = (EnemyMoveTypes)enemyMovePicker.CurrentIndex;
        GameSettingsData.predictionType = (MovementPredictionTypes)enemyPredicionPicker.CurrentIndex;
        GameSettingsData.movementPredictionData = (MovementPredictionDataTypes)enemyDataSendingPicker.CurrentIndex;

        GameSettingsData.sendRatePerSecond = int.Parse(sendRatePerSecondInput.text);
        GameSettingsData.bufferWaitTime = float.Parse(bufferWaitTimeInput.text);
        GameSettingsData.spawnNumberOfEnemies = int.Parse(spawnNumberOfEnemiesInput.text);
        GameSettingsData.simulateDropingPackages = simulateDropingPackagesToggle.isOn;
        GameSettingsData.dropEachSetNumberPackage = int.Parse(dropEachSetNumberPackageInput.text);
    }

    private GameFlowManager GameFlowManager => GameFlowManager.Instance;
    private NetworkConnectionManager NetworkConnectionManager => NetworkConnectionManager.Instance;
    private GameSettingsData GameSettingsData => GameSettingsManager.Instance.Settings;
}
