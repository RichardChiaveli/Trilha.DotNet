namespace Trilha.DotNet.Repository;

public sealed class EntityFrameworkRepository<TContext> where TContext : DbContext
{
    private readonly TContext _context;

    public EntityFrameworkRepository(TContext context)
    {
        _context = context;
    }

    public IQueryable<T> GetBy<T>(
        Expression<Func<T, bool>> arguments
        , bool asNoTracking = true
        , params Expression<Func<T, object>>[] includes) where T : class
    {
        IQueryable<T> query = _context.Set<T>();

        if (includes.HasProperties())
        {
            query = includes.Aggregate(query, (current, include) => current.Include(include));
        }

        if (arguments.HasFilter())
        {
            query = query.Where(arguments);
        }

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return query;
    }

    public async Task<T> Get<T>(object id) where T : class
        => (await _context.Set<T>().FindAsync(id))!;

    public async Task<bool> Add<T>(params T[] entities) where T : class
    {
        await _context.Set<T>().AddRangeAsync(entities);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<T> Update<T>(T entity) where T : class
    {
        var model = _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
        return model.Entity;
    }

    public async Task<bool> Update<T>(
        Expression<Func<T, bool>> arguments
        , Expression<Func<SetPropertyCalls<T>
        , SetPropertyCalls<T>>> set) where T : class
    {
        var result = await GetBy(arguments, false).ExecuteUpdateAsync(set);
        return result > 0;
    }

    public async Task<bool> Delete<T>(object id) where T : class
    {
        var entity = await Get<T>(id) ?? throw new ArgumentNullException(nameof(id));

        _context.Set<T>().Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> Delete<T>(
        Expression<Func<T, bool>> where
        , params Expression<Func<T, object>>[] includes) where T : class
    {
        var result = await GetBy(where, false, includes).ExecuteDeleteAsync();
        return result > 0;
    }
}