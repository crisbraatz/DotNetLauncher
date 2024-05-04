using Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Infrastructure.MongoDb.Mappings;

public abstract class BaseEntityMapping<T> where T : BaseEntity
{
    public abstract string CollectionName { get; }
    protected readonly IndexKeysDefinitionBuilder<T> Builder = Builders<T>.IndexKeys;
    protected BsonClassMap<T> Mapper;
    private readonly List<(IndexKeysDefinition<T> Index, bool Unique)> _indexes = [];

    protected abstract void MapProperties();
    protected abstract void MapIndexes();
    protected void AddIndex(IndexKeysDefinition<T> index, bool unique = false) => _indexes.Add((index, unique));

    protected void CreateIndexes(IMongoDatabase database)
    {
        MapIndexes();

        var indexes = _indexes
            .Select(x =>
                new CreateIndexModel<T>(x.Index, new CreateIndexOptions<T> { Background = true, Unique = x.Unique }))
            .ToList();
        if (indexes.Count is not 0)
            database.GetCollection<T>(CollectionName).Indexes.CreateMany(indexes);
    }

    protected void Map()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(T)))
            BsonClassMap.RegisterClassMap<T>(x =>
            {
                Mapper = x;
                Mapper.AutoMap();
                MapProperties();
                MapBaseProperties();
            });
    }

    private void MapBaseProperties()
    {
        Mapper.SetIdMember(Mapper
            .GetMemberMap(x => x.Id)
            .SetSerializer(new StringSerializer(BsonType.ObjectId))
            .SetIgnoreIfDefault(true));
        Mapper.MapMember(x => x.Id).SetElementName("id").SetIsRequired(true);
        Mapper.MapMember(x => x.CreatedAt).SetElementName("created_at").SetIsRequired(true);
        Mapper.MapMember(x => x.CreatedBy).SetElementName("created_by").SetIsRequired(true);
        Mapper.MapMember(x => x.UpdatedAt).SetElementName("updated_at").SetIsRequired(true);
        Mapper.MapMember(x => x.UpdatedBy).SetElementName("updated_by").SetIsRequired(true);
        Mapper.MapMember(x => x.Active).SetElementName("active").SetIsRequired(true);
        Mapper.SetIgnoreExtraElements(true);
        Mapper.SetIsRootClass(true);
    }
}