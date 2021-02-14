using MLAPI.Transports.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public static class SocketTasksExtension
{
    private const int ISDONE_MILLISECONDS_DELAY = 100;

    public async static Task IsDoneAsync(this SocketTasks socketTasks, CancellationToken ct)
    {
        try
        {
            while (!socketTasks.IsDone)
            {
                await Task.Delay(ISDONE_MILLISECONDS_DELAY, ct);
            }
        }
        catch (TaskCanceledException)
        {
            // ignore: Task.Delay throws this exception when ct.IsCancellationRequested = true
            // In this case, we only want to stop polling and finish this async Task.
        }
    }
}
