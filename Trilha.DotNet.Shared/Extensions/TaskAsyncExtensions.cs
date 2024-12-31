namespace Trilha.DotNet.Shared.Extensions;

public static class TaskAsyncExtensions
{
    public static async Task PromisseAllAsync(this IEnumerable<Task> poolTask, bool throwException = false)
    {
        try
        {
            await Task.WhenAll(poolTask);
        }
        catch (Exception ex)
        {
            if (throwException)
            {
                throw ex is AggregateException aggregateException
                    ? aggregateException.Flatten().InnerException ?? ex
                    : ex;
            }
        }
    }

    public static async Task<T[]> PromisseAllAsync<T>(this IEnumerable<Task<T>> poolTask, bool throwException = false)
    {
        try
        {
            return await Task.WhenAll(poolTask);
        }
        catch (Exception ex)
        {
            if (throwException)
            {
                throw ex is AggregateException aggregateException
                    ? aggregateException.Flatten().InnerException ?? ex
                    : ex;
            }

            return [];
        }
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