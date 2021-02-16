using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : SceneSingleton<GameFlowManager>
{
    [SerializeField] private string mainMenuSceneName = default;
    [SerializeField] private string gameMenuSceneName = default;
    [SerializeField] private string loadingSceneName = default;

    private CancellationTokenSource cts = new CancellationTokenSource();

    public async void StartServerGame()
    {
        CancellationToken ct = cts.Token;

        await ShowLoadingScene(ct);

        bool connectionSuccessfull = await NetworkConnectionManager.CreateServer(ct);

        if (!connectionSuccessfull)
        {
            await HideLoadingScene(ct);
            return;
        }

        await UnloadScene(mainMenuSceneName, ct);

        await LoadScene(gameMenuSceneName, ct);

        await HideLoadingScene(ct);
    }

    public async void StartClientGame()
    {
        CancellationToken ct = cts.Token;

        await ShowLoadingScene(ct);

        bool connectionSuccessfull = await NetworkConnectionManager.ConnectClient(ct);

        if (!connectionSuccessfull)
        {
            await HideLoadingScene(ct);
            return;
        }

        await UnloadScene(mainMenuSceneName, ct);

        await LoadScene(gameMenuSceneName, ct);

        await HideLoadingScene(ct);
    }

    public async void DisconnectAndLoadMainMenu()
    {
        CancellationToken ct = cts.Token;

        await ShowLoadingScene(ct);

        NetworkConnectionManager.Disconnect();

        await UnloadScene(gameMenuSceneName, ct);

        await LoadScene(mainMenuSceneName, ct);

        await HideLoadingScene(ct);
    }

    private async Task ShowLoadingScene(CancellationToken ct)
    {
        await LoadScene(loadingSceneName, ct);
    }

    private async Task HideLoadingScene(CancellationToken ct)
    {
        await UnloadScene(loadingSceneName, ct);
    }

    private async Task LoadScene(string sceneName, CancellationToken ct)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        await operation.AsyncCompleted(ct);
    }

    private async Task UnloadScene(string sceneName, CancellationToken ct)
    {
        AsyncOperation operation = SceneManager.UnloadSceneAsync(sceneName);

        await operation.AsyncCompleted(ct);
    }

    private NetworkConnectionManager NetworkConnectionManager => NetworkConnectionManager.Instance;
}
