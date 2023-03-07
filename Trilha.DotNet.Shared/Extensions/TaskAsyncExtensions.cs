namespace Trilha.DotNet.Shared.Extensions;

public static class TaskAsyncExtensions
{
    public static void PromisseAllAsync(this List<Task> poolTask)
    {
        while (poolTask.Find(o => o.Status != TaskStatus.RanToCompletion) != null)
            Task.WaitAll(poolTask.ToArray());
    }

    public static async Task RunByScopeAsync<TService>(
        this IServiceProvider serviceProvider, string function, params object[] args) where TService : class
    {
        IServiceScope? scope = null;

        try
        {
            scope = serviceProvider.CreateScope();
            var serviceInstance = scope.ServiceProvider.GetRequiredService<TService>();

            await (Task)typeof(TService).InvokeMember(
                function, BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod,
                null, serviceInstance, args)!;
        }
        finally
        {
            scope?.Dispose();
        }
    }
}
