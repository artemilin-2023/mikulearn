using HackBack.Application.Abstractions.Data;
using HackBack.Domain.Entities;
using HackBack.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HackBack.Infrastructure.Data.Repositories;

public class Repository<TEntity, TId> : IRepository<TEntity, TId>
        where TEntity : BaseEntity<TId>
{
    protected readonly DataContext _context;
    protected readonly DbSet<TEntity> _dbSet;
    protected readonly ILogger<Repository<TEntity, TId>> _logger;

    public Repository(DataContext context, ILogger<Repository<TEntity, TId>> logger)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
        _logger = logger;
    }

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogDebug("Entity added: {Entity}", entity);
        return entity;
    }

    public async Task<IEnumerable<TEntity>> AddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogDebug("{Count} entities added", entities.Count());
        return entities;
    }

    public IQueryable<TEntity> AsQuery(bool tracking = false)
    {
        if (tracking)
        {
            return _dbSet.AsQueryable();
        }
        return _dbSet.AsNoTracking().AsQueryable();
    }

    public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogDebug("Entity updated: {Entity}", entity);
        return entity;
    }

    public async Task DeleteAsync(TId id, CancellationToken cancellationToken)
    {
        var entity = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        if (entity == null)
        {
            throw new KeyNotFoundException($"Entity of type {typeof(TEntity).Name} with id {id} not found.");
        }
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogDebug("Entity deleted: {Id}", id);
    }
}
