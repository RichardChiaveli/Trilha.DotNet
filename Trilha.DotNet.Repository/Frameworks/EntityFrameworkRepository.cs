namespace Trilha.DotNet.Repository.Frameworks;

public class EntityFrameworkRepository<TContext>(TContext context) where TContext : DbContext
{
    protected IQueryable<T> Get<T>(
        Expression<Func<T, bool>>? where, params Expression<Func<T, object>>[] joins) where T : class
    {
        IQueryable<T> query = context.Set<T>();

        if (joins.HasProperties())
        {
            query = joins.Aggregate(query, (current, include) => current.Include(include));
        }

        if (where != null && where.HasFilter())
        {
            query = query.Where(where);
        }

        return query;
    }

    protected IQueryable<T> GetAsNoTracking<T>(
        Expression<Func<T, bool>>? where, params Expression<Func<T, object>>[] joins) where T : class
        => Get(where, joins).AsNoTracking();

    protected async Task<PagedData<T>> GetPagedData<T>(
        int currentPage = 1, int pageSize = 100, Expression<Func<T, bool>>? where = null, params Expression<Func<T, object>>[] joins) where T : class
    {
        var query = Get(where, joins);
        var totalItems = await query.CountAsync();
        var data = await query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedData<T>(data, currentPage, totalItems, pageSize);
    }

    protected async Task<PagedData<T>> GetPagedDataAsNoTracking<T>(
        int currentPage = 1, int pageSize = 100, Expression<Func<T, bool>>? where = null, params Expression<Func<T, object>>[] joins) where T : class
    {
        var query = Get(where, joins).AsNoTracking();
        var totalItems = await query.CountAsync();
        var data = await query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedData<T>(data, currentPage, totalItems, pageSize);
    }

    protected async Task<T> Get<T>(object id) where T : class
        => await context.Set<T>().FindAsync(id) ?? throw new KeyNotFoundException($"Entity with ID {id} not found.");

    protected async Task Add<T>(params T[] entities) where T : class
    {
        await context.Set<T>().AddRangeAsync(entities);
    }

    protected EntityEntry<T> Update<T>(T entity) where T : class 
        => context.Set<T>().Update(entity);

    protected async Task Update<T>(
        Expression<Func<T, bool>>? arguments
        , Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> set) where T : class
    {
        await Get(arguments).ExecuteUpdateAsync(set);
    }

    protected async Task Delete<T>(object id) where T : class
    {
        var entity = await Get<T>(id) ?? throw new ArgumentNullException(nameof(id));

        context.Set<T>().Remove(entity);
    }

    protected async Task Delete<T>(Expression<Func<T, bool>>? where) where T : class
    {
        await Get(where).ExecuteDeleteAsync();
    }

    protected async Task<bool> Commit()
    {
        try
        {
            return await context.SaveChangesAsync() > 0;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new InvalidOperationException("A concurrency error occurred while saving changes.", ex);
        }
    }

    protected async Task<bool> CommitTransactionAsync(Func<Task> action)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            await action();
            await transaction.CommitAsync();

            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new InvalidOperationException("An error occurred during the transaction.", ex);
        }
    }
}