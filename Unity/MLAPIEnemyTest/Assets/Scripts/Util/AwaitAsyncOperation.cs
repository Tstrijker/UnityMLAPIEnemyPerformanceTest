using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


public static class AwaitAsyncOperation
{
    public async static Task AsyncCompleted(this AsyncOperation asyncOperation, CancellationToken ct)
    {
        try
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            ct.Register(()=> tcs.TrySetCanceled());

            asyncOperation.completed += (AsyncOperation operation) =>
            {
                tcs.SetResult(true);
            };

            await tcs.Task;
        }
        catch (TaskCanceledException)
        {
            // ignore: Task.Delay throws this exception when ct.IsCancellationRequested = true
            // In this case, we only want to stop polling and finish this async Task.
        }
    }
}

