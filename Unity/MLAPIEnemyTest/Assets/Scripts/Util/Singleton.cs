using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private const int WAITFORLOADED_MILLISECONDS_DELAY = 100;

    private static T instance;
    private static bool quitting;
    private static readonly object instanceLock = new object();
    private static WaitUntil waitUntil = null;

    public static bool Exists { get; private set; }

    public static T Instance
    {
        get
        {
            if (quitting)
            {
                Debug.LogWarning("SingleSceneSingleton::Instance is not loaded yet.");
                return null;
            }
            lock (instanceLock)
            {
                if (instance != null)
                    return instance;

                T[] instances = FindObjectsOfType<T>();

                if (instances.Length > 0)
                {
                    instance = instances[0];

                    DontDestroyOnLoad(instance);

                    if (instances.Length > 1)
                        Debug.LogError($"There are {instances.Length} instances of the singleton {typeof(T)}, please make sure that there is only one.");
                }
                else 
                {
                    GameObject singletonObj = new GameObject();

                    singletonObj.name = $"(Singleton) {typeof(T)}";

                    instance = singletonObj.AddComponent<T>();

                    DontDestroyOnLoad(instance);
                }

                Exists = true;

                return instance;
            }
        }
    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            quitting = true;

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