using Domain.DTOs;
using FizzWare.NBuilder;
using FluentAssertions;

namespace Unit.Domain.DTOs;

public class BaseListResponseDtoTests
{
    private ListDtoForTests _dto;

    [Theory]
    [InlineData(10, 1, 10, 10, 1)]
    [InlineData(10, 2, 10, 20, 2)]
    [InlineData(10, 3, 10, 31, 4)]
    public void Should_create_dto_with_many_data(
        int listSize, int currentPage, int size, int totalItems, int expectedTotalPages)
    {
        _dto = new ListDtoForTests(
            Builder<DtoForTests>.CreateListOfSize(listSize).Build().ToList(), currentPage, size, totalItems);

        _dto.Data.Should().HaveCount(listSize);
        _dto.CurrentPage.Should().Be(currentPage);
        _dto.TotalPages.Should().Be(expectedTotalPages);
        _dto.TotalItems.Should().Be(totalItems);
    }

    [Fact]
    public void Should_create_dto_with_single_data()
    {
        _dto = new ListDtoForTests(new DtoForTests());

        _dto.Data.Should().HaveCount(1);
        _dto.CurrentPage.Should().Be(1);
        _dto.TotalPages.Should().Be(1);
        _dto.TotalItems.Should().Be(1);
    }

    [Fact]
    public void Should_create_empty_dto()
    {
        _dto = new ListDtoForTests();

        _dto.Data.Should().BeEmpty();
        _dto.CurrentPage.Should().Be(1);
        _dto.TotalPages.Should().Be(1);
        _dto.TotalItems.Should().Be(0);
    }

    private class ListDtoForTests : BaseListResponseDto<DtoForTests>
    {
        public ListDtoForTests(IEnumerable<DtoForTests> data, int currentPage, int size, int totalItems)
            : base(data, currentPage, size, totalItems)
        {
        }

        public ListDtoForTests(DtoForTests data) : base(data)
        {
        }

        public ListDtoForTests()
        {
        }
    }

    private class DtoForTests;
}