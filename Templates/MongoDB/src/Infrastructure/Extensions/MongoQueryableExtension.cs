using System.Linq.Expressions;
using System.Reflection;
using Domain;
using MongoDB.Driver.Linq;

namespace Infrastructure.Extensions;

public static class MongoQueryableExtension
{
    public static IMongoQueryable<T> OrderBy<T>(this IMongoQueryable<T> queryable, string propertyName, bool ascending)
        where T : class
    {
        if (propertyName is null)
            return queryable;

        var entityType = typeof(T);

        var propertyInfo = entityType.GetProperty(
            AppSettings.AppLanguage.TextInfo.ToTitleCase(propertyName),
            BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
        if (propertyInfo is null)
            return queryable;

        var methodInfo = typeof(Queryable).GetMethods()
            .Where(x => x.Name == (ascending ? "OrderBy" : "OrderByDescending") && x.IsGenericMethodDefinition)
            .Single(x => x.GetParameters().ToList().Count is 2)
            .MakeGenericMethod(entityType, propertyInfo.PropertyType);

        var parameterExpression = Expression.Parameter(entityType, "x");
        var memberExpression = Expression.Property(parameterExpression, propertyName);
        var lambdaExpression = Expression.Lambda(memberExpression, parameterExpression);

        return methodInfo.Invoke(methodInfo, [queryable, lambdaExpression]) as IMongoQueryable<T>;
    }

    public static IMongoQueryable<T> PaginateBy<T>(this IMongoQueryable<T> queryable, int page, int size) =>
        queryable.Skip((page - 1) * size).Take(size);
}