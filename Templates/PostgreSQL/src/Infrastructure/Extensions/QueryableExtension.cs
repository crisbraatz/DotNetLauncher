using System.Linq.Expressions;
using System.Reflection;
using Domain;

namespace Infrastructure.Extensions;

public static class QueryableExtension
{
    public static IQueryable<T> OrderBy<T>(this IQueryable<T> queryable, string propertyName, bool ascending)
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

        return methodInfo.Invoke(methodInfo, [queryable, lambdaExpression]) as IQueryable<T>;
    }

    public static IQueryable<T> PaginateBy<T>(this IQueryable<T> queryable, int page, int size) =>
        queryable.Skip((page - 1) * size).Take(size);
}