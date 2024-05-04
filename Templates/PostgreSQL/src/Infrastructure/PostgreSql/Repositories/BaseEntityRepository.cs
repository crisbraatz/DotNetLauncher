using System.Linq.Expressions;
using Domain.DTOs;
using Domain.Entities;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.PostgreSql.Repositories;

public class BaseEntityRepository<T>(DatabaseContext context) : IBaseEntityRepository<T> where T : BaseEntity
{
    private readonly DbSet<T> _entity = context.Set<T>();

    public void DeleteMany(IEnumerable<T> entities) => UpdateMany(entities);

    public void DeleteOne(T entity) => UpdateOne(entity);

    public async Task<bool> ExistsByAsync(Expression<Func<T, bool>> filter = null, CancellationToken token = default) =>
        await _entity.AsNoTracking().AnyAsync(filter ?? (x => true), token);

    public async Task InsertManyAsync(IEnumerable<T> entities, CancellationToken token = default) =>
        await _entity.AddRangeAsync(entities, token);

    public async Task InsertOneAsync(T entity, CancellationToken token = default) =>
        await _entity.AddAsync(entity, token);

    public async Task<(List<TP> data, int total)> ListByAsync<TP>(
        BaseListRequestDto<T> request, Expression<Func<T, TP>> project, CancellationToken token = default)
        where TP : BaseDataForListResponseDto
    {
        var queryable = _entity.AsQueryable();

        if (request.Filters.Count is not 0)
            queryable = request.Filters.Aggregate(queryable, (current, filter) => current.Where(filter));

        if (request.OrderBy.Any())
            foreach (var (propertyName, ascending) in request.OrderBy)
                queryable = queryable.OrderBy(propertyName, ascending);

        queryable = queryable.AsNoTracking();

        var data = await queryable.PaginateBy(request.Page, request.Size).Select(project).ToListAsync(token);

        var total = await queryable.CountAsync(token);

        return (data, total);
    }

    public async Task<IList<TP>> ProjectManyByAsync<TP>(
        Expression<Func<T, TP>> project, Expression<Func<T, bool>> filter = null, CancellationToken token = default) =>
        await _entity.AsNoTracking().Where(filter ?? (x => true)).Select(project).ToListAsync(token);

    public async Task<TP> ProjectOneByAsync<TP>(
        Expression<Func<T, TP>> project, Expression<Func<T, bool>> filter = null, CancellationToken token = default) =>
        await _entity.AsNoTracking().Where(filter ?? (x => true)).Select(project).SingleOrDefaultAsync(token);

    public async Task<IList<T>> SelectManyByAsync(
        Expression<Func<T, bool>> filter = null, bool track = false, CancellationToken token = default) =>
        track
            ? await _entity.Where(filter ?? (x => true)).ToListAsync(token)
            : await _entity.AsNoTracking().Where(filter ?? (x => true)).ToListAsync(token);

    public async Task<T> SelectOneByAsync(
        Expression<Func<T, bool>> filter = null, bool track = false, CancellationToken token = default) =>
        track
            ? await _entity.SingleOrDefaultAsync(filter ?? (x => true), token)
            : await _entity.AsNoTracking().SingleOrDefaultAsync(filter ?? (x => true), token);

    public void UpdateMany(IEnumerable<T> entities) => _entity.UpdateRange(entities);

    public void UpdateOne(T entity) => _entity.Update(entity);
}