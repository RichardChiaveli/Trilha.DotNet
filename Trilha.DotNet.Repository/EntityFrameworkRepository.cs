namespace Trilha.DotNet.Repository;

public abstract class EntityFrameworkRepository
{
    private readonly DbContext _context;
    public readonly IDbConnection Connection;

    protected EntityFrameworkRepository(DbContext context)
    {
        _context = context;
        Connection = context.Database.GetDbConnection();
    }

    public IQueryable<TEntity> GetBy<TEntity>(Expression<Func<TEntity, bool>> arguments, bool asNoTracking = true,
        params Expression<Func<TEntity, object>>[] includes) where TEntity : class
    {
        IQueryable<TEntity> query = _context.Set<TEntity>();

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

    public async Task<TEntity> Get<TEntity>(object id) where TEntity : class
        => (await _context.Set<TEntity>().FindAsync(id))!;

    public async Task<bool> Create<TEntity>(params TEntity[] entities) where TEntity : class
    {
        await _context.Set<TEntity>().AddRangeAsync(entities);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<TEntity> Update<TEntity>(TEntity entity) where TEntity : class
    {
        var model = _context.Set<TEntity>().Update(entity);
        await _context.SaveChangesAsync();
        return model.Entity;
    }

    public async Task<bool> Update<TEntity>(Expression<Func<TEntity, bool>> arguments, Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> set) where TEntity : class
    {
        var result = await GetBy(arguments, false).ExecuteUpdateAsync(set);
        return result > 0;
    }

    public async Task<bool> Delete<TEntity>(object id) where TEntity : class
    {
        var entity = await Get<TEntity>(id);

        if (entity == null)
            throw new ArgumentNullException(nameof(id));

        _context.Set<TEntity>().Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> Delete<TEntity>(Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] includes) where TEntity : class
    {
        var result = await GetBy(where, false, includes).ExecuteDeleteAsync();
        return result > 0;
    }
}