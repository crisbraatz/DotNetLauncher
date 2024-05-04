using System.Reflection;
using Domain;
using Infrastructure.MongoDb.Mappings;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.MongoDb;

public class DatabaseHelper
{
    public readonly IMongoDatabase Database;
    private static bool _isMapped;

    public DatabaseHelper()
    {
        Database = new MongoClient(AppSettings.MongoDbConnectionString).GetDatabase(AppSettings.MongoDbDatabase);

        Setup(Database);
    }

    private static void Setup(IMongoDatabase database)
    {
        var mappingTypes = Assembly.GetExecutingAssembly().GetTypes().Where(x =>
            typeof(BaseEntityMapping<>).IsAssignableFrom(x) &&
            x is { IsAbstract: false, IsInterface: false } &&
            x != typeof(BaseEntityMapping<>));
        foreach (var mappingType in mappingTypes)
        {
            var instance = Activator.CreateInstance(mappingType);

            CreateCollectionIfNotExist(database, instance);

            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public;

            if (!_isMapped)
                mappingType.InvokeMember("Map", bindingFlags, null, instance, null);

            mappingType.InvokeMember("CreateIndexes", bindingFlags, null, instance, [database]);
        }

        _isMapped = true;
    }

    private static void CreateCollectionIfNotExist(IMongoDatabase database, object mapping)
    {
        var collectionName = mapping
            .GetType()
            .GetProperty("CollectionName", BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public)
            ?.GetValue(mapping) as string;

        var collectionExist = database
            .ListCollectionNames(new ListCollectionNamesOptions { Filter = new BsonDocument("name", collectionName) })
            .Any();

        if (!string.IsNullOrWhiteSpace(collectionName) && !collectionExist)
            database.CreateCollection(collectionName);
    }
}