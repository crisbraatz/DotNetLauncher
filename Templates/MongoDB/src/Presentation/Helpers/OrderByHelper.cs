namespace Presentation.Helpers;

public static class OrderByHelper
{
    public static IDictionary<string, bool> ToDictionary<T>(string orderBy)
    {
        var propertiesName = typeof(T).GetProperties().Select(x => x.Name.ToLowerInvariant()).ToHashSet();

        orderBy ??= $"{propertiesName.FirstOrDefault()} asc";

        var dictionary = new Dictionary<string, bool>();

        foreach (var value in orderBy.Split(';'))
        {
            var data = value.Split(' ');

            var propertyName = data.FirstOrDefault()?.Trim().ToLowerInvariant();

            if (!string.IsNullOrWhiteSpace(propertyName) && propertiesName.Contains(propertyName))
                dictionary.Add(propertyName, data.LastOrDefault()?.Trim().ToLowerInvariant() switch
                {
                    "asc" => true,
                    "desc" => false,
                    _ => true
                });
        }

        return dictionary;
    }
}