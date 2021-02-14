using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public abstract class SceneSingleton<T> : MonoBehaviour where T : SceneSingleton<T>
{
    private const int WAITFORLOADED_MILLISECONDS_DELAY = 100;

    private static T instance;
    private static bool showedWarning = false;
    private static WaitUntil waitUntil = null;

    public static bool Exists { get; private set; }

    public static T Instance
    {
        get
        {
            if (!Exists && Application.isPlaying && !showedWarning)
            {
                Debug.LogWarning("SingleSceneSingleton::Instance is not loaded yet.");
                showedWarning = true;
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance != null)
        {
            if (instance != (T)this)
            {
                Debug.LogWarning($"It look like a second instance is being loaded as a singleton called: {this.gameObject.scene}", this);
            }

            return;
        }

        instance = (T)this;
        Exists = instance != null;
    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
            Exists = false;
        }
    }

    public static WaitUntil WaitForLoaded()
    {
        if (waitUntil == null)
        {
            waitUntil = new WaitUntil(() =>
            {
                return Exists;
            });
        }

        return waitUntil;
    }

    public static async Task AsyncWaitForLoaded(CancellationToken ct)
    {
        while (!Exists)
        {
            await Task.Delay(WAITFORLOADED_MILLISECONDS_DELAY, ct);
        }
    }
}