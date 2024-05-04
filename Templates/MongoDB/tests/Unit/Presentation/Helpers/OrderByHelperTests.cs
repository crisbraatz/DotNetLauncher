using FluentAssertions;
using Presentation.Helpers;

namespace Unit.Presentation.Helpers;

public class OrderByHelperTests
{
    [Theory, MemberData(nameof(CasesOrderBy))]
    public void Should_convert_non_empty_order_by_string_to_non_empty_order_by_dictionary(
        string sentString, IDictionary<string, bool> expectedDictionary)
    {
        var returnedDictionary = OrderByHelper.ToDictionary<DtoForTests>(sentString);

        returnedDictionary.Should().BeEquivalentTo(expectedDictionary);
    }

    [Fact]
    public void Should_convert_empty_order_by_string_to_empty_order_by_dictionary()
    {
        var dictionary = OrderByHelper.ToDictionary<DtoForTests>(string.Empty);

        dictionary.Should().BeEquivalentTo(new Dictionary<string, bool>());
    }

    [Fact]
    public void Should_convert_null_order_by_string_to_default_order_by_dictionary()
    {
        var dictionary = OrderByHelper.ToDictionary<DtoForTests>(null);

        dictionary.Should().BeEquivalentTo(new Dictionary<string, bool> { { "firstproperty", true } });
    }

    public static IEnumerable<object[]> CasesOrderBy() => new List<object[]>
    {
        new object[] { "FIRSTPROPERTY ASC", new Dictionary<string, bool> { { "firstproperty", true } } },
        new object[] { "secondproperty desc", new Dictionary<string, bool> { { "secondproperty", false } } },
        new object[]
        {
            "FirstProperty ASC;SecondProperty DESC",
            new Dictionary<string, bool> { { "firstproperty", true }, { "secondproperty", false } }
        },
        new object[]
        {
            "SecondProperty DESC;FirstProperty ASC",
            new Dictionary<string, bool> { { "secondproperty", false }, { "firstproperty", true } }
        }
    };

    private class DtoForTests
    {
        public string FirstProperty { get; init; }
        public string SecondProperty { get; init; }
    }
}