using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.PostgreSql.Mappings;

public class UserMapping(EntityTypeBuilder<User> builder) : BaseEntityMapping<User>(builder)
{
    protected override string TableName => "users";

    protected override void MapProperties()
    {
        Builder.Property(x => x.Email).HasColumnName("email").IsRequired();
        Builder.Property(x => x.Password).HasColumnName("password").IsRequired();
    }

    protected override void MapIndexes() => Builder.HasIndex(x => new { x.Email, x.Active });

    protected override void MapForeignKeys()
    {
    }
}