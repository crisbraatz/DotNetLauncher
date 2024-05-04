using Domain.DTOs;
using Domain.Entities;
using FluentAssertions;

namespace Unit.Domain.DTOs;

public class BaseListRequestDtoTests
{
    private DtoForTests _dto;

    [Theory]
    [InlineData(1, 1, 1, 1)]
    [InlineData(2, 2, 2, 2)]
    [InlineData(0, 0, 1, 10)]
    [InlineData(-1, -1, 1, 10)]
    [InlineData(10, 100, 10, 100)]
    [InlineData(10, 101, 10, 100)]
    public void Should_create_dto_with_custom_values(int sentPage, int sentSize, int expectedPage, int expectedSize)
    {
        const string requestedBy = "RequestedBy";
        var token = new CancellationToken();
        var id = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;
        const string createdBy = "CreatedBy";
        var updatedAt = DateTime.UtcNow;
        const string updatedBy = "UpdatedBy";
        const bool active = true;

        _dto = new DtoForTests(requestedBy, token)
        {
            Filters = [x => x.Active],
            OrderBy = new Dictionary<string, bool> { { "id", true } },
            Page = sentPage,
            Size = sentSize,
            Id = id,
            CreatedAt = createdAt,
            CreatedBy = createdBy,
            UpdatedAt = updatedAt,
            UpdatedBy = updatedBy,
            Active = active
        };

        _dto.Filters.Should().HaveCount(1);
        _dto.OrderBy.Should().HaveCount(1);
        _dto.RequestedBy.Should().Be(requestedBy);
        _dto.Token.Should().Be(token);
        _dto.Page.Should().Be(expectedPage);
        _dto.Size.Should().Be(expectedSize);
        _dto.Id.Should().Be(id);
        _dto.CreatedAt.Should().Be(createdAt);
        _dto.CreatedBy.Should().Be(createdBy);
        _dto.UpdatedAt.Should().Be(updatedAt);
        _dto.UpdatedBy.Should().Be(updatedBy);
        _dto.Active.Should().Be(active);
    }

    [Fact]
    public void Should_create_dto_with_default_values()
    {
        _dto = new DtoForTests();

        _dto.Filters.Should().BeEmpty();
        _dto.OrderBy.Should().BeEmpty();
        _dto.RequestedBy.Should().BeNull();
        _dto.Token.Should().BeEquivalentTo(new CancellationToken());
        _dto.Page.Should().Be(1);
        _dto.Size.Should().Be(10);
        _dto.Id.Should().BeNull();
        _dto.CreatedAt.Should().BeNull();
        _dto.CreatedBy.Should().BeNull();
        _dto.UpdatedAt.Should().BeNull();
        _dto.UpdatedBy.Should().BeNull();
        _dto.Active.Should().BeNull();
    }

    private class DtoForTests(string requestedBy = null, CancellationToken token = default)
        : BaseListRequestDto<EntityForTests>(requestedBy, token);

    private class EntityForTests : BaseEntity;
}