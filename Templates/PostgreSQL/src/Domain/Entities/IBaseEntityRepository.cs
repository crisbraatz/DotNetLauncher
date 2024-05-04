using System.Linq.Expressions;
using Domain.DTOs;

namespace Domain.Entities;

public interface IBaseEntityRepository<T> where T : BaseEntity
{
    void DeleteMany(IEnumerable<T> entities);
    void DeleteOne(T entity);
    Task<bool> ExistsByAsync(Expression<Func<T, bool>> filter = null, CancellationToken token = default);
    Task InsertManyAsync(IEnumerable<T> entities, CancellationToken token = default);
    Task InsertOneAsync(T entity, CancellationToken token = default);

    Task<(List<TP> data, int total)> ListByAsync<TP>(
        BaseListRequestDto<T> request, Expression<Func<T, TP>> project, CancellationToken token = default)
        where TP : BaseDataForListResponseDto;

    Task<IList<TP>> ProjectManyByAsync<TP>(
        Expression<Func<T, TP>> project, Expression<Func<T, bool>> filter = null, CancellationToken token = default);

    Task<TP> ProjectOneByAsync<TP>(
        Expression<Func<T, TP>> project, Expression<Func<T, bool>> filter = null, CancellationToken token = default);

    Task<IList<T>> SelectManyByAsync(
        Expression<Func<T, bool>> filter = null, bool track = false, CancellationToken token = default);

    Task<T> SelectOneByAsync(
        Expression<Func<T, bool>> filter = null, bool track = false, CancellationToken token = default);

    void UpdateMany(IEnumerable<T> entities);
    void UpdateOne(T entity);
}