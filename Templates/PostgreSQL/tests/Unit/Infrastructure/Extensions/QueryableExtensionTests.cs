using FizzWare.NBuilder;
using FluentAssertions;
using Infrastructure.Extensions;

namespace Unit.Infrastructure.Extensions;

public class QueryableExtensionTests
{
    private readonly List<DtoForTests> _dto = Builder<DtoForTests>
        .CreateListOfSize(10)
        .All()
        .Do(x => x.Property = Guid.NewGuid().ToString())
        .Build()
        .ToList();

    private readonly List<int> _intList = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
    private const string PropertyName = "Property";

    [Fact]
    public void Should_get_ascending_ordered_queryable()
    {
        var expectedList = _dto.OrderBy(x => x.Property);

        var returnedList = _dto.AsQueryable().OrderBy(PropertyName, true);

        returnedList?.SequenceEqual(expectedList).Should().BeTrue();
    }

    [Fact]
    public void Should_get_descending_ordered_queryable()
    {
        var expectedList = _dto.OrderByDescending(x => x.Property);

        var returnedList = _dto.AsQueryable().OrderBy(PropertyName, false);

        returnedList?.SequenceEqual(expectedList).Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("InvalidProperty")]
    public void Should_get_unordered_queryable_when_invalid_property_provided(string property)
    {
        var expectedList = _dto.OrderBy(x => x.Property);

        var returnedList = _dto.AsQueryable().OrderBy(property, true);

        returnedList?.SequenceEqual(expectedList).Should().BeFalse();
    }

    [Theory]
    [InlineData(1, 5, 5)]
    [InlineData(1, 10, 10)]
    [InlineData(1, 20, 10)]
    [InlineData(2, 5, 5)]
    [InlineData(2, 10, 0)]
    [InlineData(3, 5, 0)]
    public void Should_get_paginated_queryable(int page, int sentSize, int expectedSize)
    {
        var list = _intList.AsQueryable().PaginateBy(page, sentSize);

        list.Should().HaveCount(expectedSize);
    }

    private class DtoForTests
    {
        public string Property { get; set; }
    }
}