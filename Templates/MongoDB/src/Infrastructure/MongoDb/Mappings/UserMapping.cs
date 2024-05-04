using Domain.Entities.Users;

namespace Infrastructure.MongoDb.Mappings;

public class UserMapping : BaseEntityMapping<User>
{
    public override string CollectionName => "users";

    protected override void MapProperties()
    {
        Mapper.MapMember(x => x.Email).SetElementName("email").SetIsRequired(true);
        Mapper.MapMember(x => x.Password).SetElementName("password").SetIsRequired(true);
    }

    protected override void MapIndexes() => AddIndex(Builder.Ascending(x => new { x.Email, x.Active }));
}