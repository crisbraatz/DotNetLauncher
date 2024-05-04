using Domain.Entities;
using FluentAssertions;

namespace Unit.Domain.Entities;

public class BaseEntityTests
{
    private const string CreateProperty = "CreateProperty";
    private const string CreateRequestedBy = "CreateRequestedBy";
    private const string UpdateRequestedBy = "UpdateRequestedBy";
    private readonly DateTime _dateTime = DateTime.UtcNow;
    private readonly EntityForTests _entity = new(CreateProperty, CreateRequestedBy);

    [Fact]
    public void Should_create_an_entity()
    {
        _entity.Property.Should().Be(CreateProperty);
        _entity.Id.Should().NotBeEmpty();
        _entity.CreatedAt.Should().BeCloseTo(_dateTime, TimeSpan.FromSeconds(1));
        _entity.CreatedBy.Should().Be(CreateRequestedBy);
        _entity.UpdatedAt.Should().BeCloseTo(_dateTime, TimeSpan.FromSeconds(1));
        _entity.UpdatedBy.Should().Be(CreateRequestedBy);
        _entity.Active.Should().BeTrue();
    }

    [Fact]
    public void Should_deactivate_an_entity()
    {
        Thread.Sleep(100);

        _entity.Deactivate(UpdateRequestedBy);

        _entity.Property.Should().Be(CreateProperty);
        _entity.Id.Should().NotBeEmpty();
        _entity.CreatedAt.Should().BeCloseTo(_dateTime, TimeSpan.FromSeconds(1));
        _entity.CreatedBy.Should().Be(CreateRequestedBy);
        _entity.UpdatedAt.Should().BeAfter(_entity.CreatedAt);
        _entity.UpdatedBy.Should().Be(UpdateRequestedBy);
        _entity.Active.Should().BeFalse();
    }

    [Fact]
    public void Should_reactivate_an_entity()
    {
        _entity.Deactivate(UpdateRequestedBy);
        Thread.Sleep(100);

        _entity.Reactivate(UpdateRequestedBy);

        _entity.Property.Should().Be(CreateProperty);
        _entity.Id.Should().NotBeEmpty();
        _entity.CreatedAt.Should().BeCloseTo(_dateTime, TimeSpan.FromSeconds(1));
        _entity.CreatedBy.Should().Be(CreateRequestedBy);
        _entity.UpdatedAt.Should().BeAfter(_entity.CreatedAt);
        _entity.UpdatedBy.Should().Be(UpdateRequestedBy);
        _entity.Active.Should().BeTrue();
    }

    [Fact]
    public void Should_update_an_entity()
    {
        const string updateProperty = "UpdateProperty";
        Thread.Sleep(100);

        _entity.UpdateProperty(updateProperty, UpdateRequestedBy);

        _entity.Property.Should().Be(updateProperty);
        _entity.Id.Should().NotBeEmpty();
        _entity.CreatedAt.Should().BeCloseTo(_dateTime, TimeSpan.FromSeconds(1));
        _entity.CreatedBy.Should().Be(CreateRequestedBy);
        _entity.UpdatedAt.Should().BeAfter(_entity.CreatedAt);
        _entity.UpdatedBy.Should().Be(UpdateRequestedBy);
        _entity.Active.Should().BeTrue();
    }

    private class EntityForTests : BaseEntity
    {
        public string Property { get; private set; }

        public EntityForTests(string property, string requestedBy)
        {
            SetCreate(requestedBy);
            Property = property;
        }

        public void Deactivate(string requestedBy) => SetInactive(requestedBy);

        public void Reactivate(string requestedBy) => SetActive(requestedBy);

        public void UpdateProperty(string property, string requestedBy)
        {
            SetUpdate(requestedBy);
            Property = property;
        }
    }
}