using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Transports.UNET;
using System.Threading.Tasks;
using System.Threading;
using MLAPI.Transports.Tasks;

public class NetworkConnectionManager : SceneSingleton<NetworkConnectionManager>
{
    [SerializeField] private NetworkingManager networkingManager;
    [SerializeField] private UnetTransport unetTransport;

    public NetworkConnectionData CurrentConnectionData { get; set; } = new NetworkConnectionData();

    public async Task<bool> CreateServer(CancellationToken ct)
    {
        unetTransport.ConnectAddress = CurrentConnectionData.connectAddress;
        unetTransport.ServerListenPort = CurrentConnectionData.connectPort;

        SocketTasks socketTasks = networkingManager.StartHost();

        await socketTasks.IsDoneAsync(ct);

        if (!socketTasks.Success)
        {
            ErrorHandler(socketTasks);
            return false;
        }

        return true;
    }

    public async Task<bool> ConnectClient(CancellationToken ct)
    {
        unetTransport.ConnectAddress = CurrentConnectionData.connectAddress;
        unetTransport.ConnectPort = CurrentConnectionData.connectPort;

        SocketTasks socketTasks = networkingManager.StartClient();

        await socketTasks.IsDoneAsync(ct);

        if (!socketTasks.Success)
        {
            ErrorHandler(socketTasks);
            return false;
        }

        return true;
    }

    public void Disconnect()
    {
        if (networkingManager.IsHost)
        {
            networkingManager.StopHost();
        }
        else if (networkingManager.IsClient)
        {
            networkingManager.StopClient();
        }
    }

    // Error handler
    private void ErrorHandler(SocketTasks socketTasks)
    {
        foreach (SocketTask task in socketTasks.Tasks)
        {
            if (!task.Success)
            {
                Debug.LogException(task.TransportException);
                Debug.LogError(task.SocketError.ToString());
            }
        }
    }
}
