using System.Linq.Expressions;
using Domain.DTOs;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.MongoDb.Mappings;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Infrastructure.MongoDb.Repositories;

public abstract class BaseEntityRepository<T, TM>(IMongoDatabase database) : IBaseEntityRepository<T>
    where T : BaseEntity
    where TM : BaseEntityMapping<T>
{
    private readonly IMongoCollection<T> _collection = database.GetCollection<T>(
        typeof(TM).GetProperty("CollectionName")?.GetValue(Activator.CreateInstance(typeof(TM))) as string);

    public async Task DeleteManyAsync(IEnumerable<T> entities, CancellationToken token = default) =>
        await UpdateManyAsync(entities, token);

    public async Task DeleteOneAsync(T entity, CancellationToken token = default) =>
        await UpdateOneAsync(entity, token);

    public async Task<bool> ExistsByAsync(Expression<Func<T, bool>> filter = null, CancellationToken token = default) =>
        await _collection.CountDocumentsAsync(filter ?? (x => true), cancellationToken: token) > 0;

    public async Task InsertManyAsync(IEnumerable<T> entities, CancellationToken token = default) =>
        await _collection.InsertManyAsync(entities, cancellationToken: token);

    public async Task InsertOneAsync(T entity, CancellationToken token = default) =>
        await _collection.InsertOneAsync(entity, cancellationToken: token);

    public async Task<(List<TP> data, int total)> ListByAsync<TP>(
        BaseListRequestDto<T> request, Expression<Func<T, TP>> project, CancellationToken token = default)
        where TP : BaseDataForListResponseDto
    {
        var queryable = _collection.AsQueryable();

        if (request.Filters.Count is not 0)
            queryable = request.Filters
                .Aggregate(queryable, (current, filter) => Queryable.Where(current, filter) as IMongoQueryable<T>);

        if (request.OrderBy.Any())
            foreach (var (propertyName, ascending) in request.OrderBy)
                queryable = queryable.OrderBy(propertyName, ascending);

        var data = await queryable.PaginateBy(request.Page, request.Size).Select(project).ToListAsync(token);

        var total = await queryable.CountAsync(token);

        return (data, total);
    }

    public async Task<IList<TP>> ProjectManyByAsync<TP>(
        Expression<Func<T, TP>> project, Expression<Func<T, bool>> filter = null, CancellationToken token = default) =>
        await _collection.Find(filter ?? (x => true)).Project(project).ToListAsync(token);

    public async Task<TP> ProjectOneByAsync<TP>(
        Expression<Func<T, TP>> project, Expression<Func<T, bool>> filter = null, CancellationToken token = default) =>
        await _collection.Find(filter ?? (x => true)).Project(project).SingleOrDefaultAsync(token);

    public async Task<IList<T>> SelectManyByAsync(
        Expression<Func<T, bool>> filter = null, CancellationToken token = default) =>
        await _collection.Find(filter ?? (x => true)).ToListAsync(token);

    public async Task<T> SelectOneByAsync(Expression<Func<T, bool>> filter = null, CancellationToken token = default) =>
        await _collection.Find(filter ?? (x => true)).SingleOrDefaultAsync(token);

    public async Task UpdateManyAsync(IEnumerable<T> entities, CancellationToken token = default) =>
        await _collection.BulkWriteAsync(
            entities
                .Select(x => new ReplaceOneModel<T>(Builders<T>.Filter.Where(y => y.Id == x.Id), x))
                .Cast<WriteModel<T>>()
                .ToList(),
            cancellationToken: token);

    public async Task UpdateOneAsync(T entity, CancellationToken token = default) =>
        await _collection.ReplaceOneAsync(x => x.Id == entity.Id, entity, cancellationToken: token);
}