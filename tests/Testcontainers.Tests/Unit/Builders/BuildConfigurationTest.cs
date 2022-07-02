namespace DotNet.Testcontainers.Tests.Unit
{
  using System.Collections.Generic;
  using System.Linq;
  using DotNet.Testcontainers.Builders;
  using Xunit;

  public class BuildConfigurationTest
  {
    public static IEnumerable<object[]> DictionaryCombinations { get; }
      = new[]
      {
        new[]
        {
          null,
          null,
          (Dictionary<string, string>)null,
        },
        new[]
        {
          null,
          new Dictionary<string, string> { ["A"] = "A" },
          new Dictionary<string, string> { ["A"] = "A" },
        },
        new[]
        {
          new Dictionary<string, string> { ["B"] = "B" },
          null,
          new Dictionary<string, string> { ["B"] = "B" },
        },
        new[]
        {
          new Dictionary<string, string> { ["A"] = "new" },
          new Dictionary<string, string> { ["A"] = "old", ["B"] = "B" },
          new Dictionary<string, string> { ["A"] = "new", ["B"] = "B" },
        },
      };

    [Theory]
    [InlineData(null, null, null)]
    [InlineData("B", null, "B")]
    [InlineData(null, "A", "A")]
    [InlineData("B", "A", "B")]
    public void CombinesReferenceTypes(string next, string previous, string expected)
    {
      // Given
      // When
      var actual = BuildConfiguration.Combine(next, previous);

      // Then
      Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(null, null, null)]
    [InlineData(new[] { "2" }, null, new[] { "2" })]
    [InlineData(null, new[] { "1" }, new[] { "1" })]
    [InlineData(new[] { "2" }, new[] { "1" }, new[] { "1", "2" })]
    [InlineData(new[] { "2", "3", "4" }, new[] { "1", "2", "3" }, new[] { "1", "2", "2", "3", "3", "4" })]
    public void CombinesEnumerables(IEnumerable<string> next, IEnumerable<string> previous, IEnumerable<string> expected)
    {
      // Given
      // When
      var actual = BuildConfiguration.Combine(next, previous);

      // Then
      Assert.Equal(expected?.OrderBy(x => x), actual?.OrderBy(x => x));
    }

    [Theory]
    [InlineData(null, null, null)]
    [InlineData(new[] { "2" }, null, new[] { "2" })]
    [InlineData(null, new[] { "1" }, new[] { "1" })]
    [InlineData(new[] { "2" }, new[] { "1" }, new[] { "1", "2" })]
    [InlineData(new[] { "2", "3", "4" }, new[] { "1", "2", "3" }, new[] { "1", "2", "3", "2", "3", "4" })]
    public void CombinesReadOnlyLists(IReadOnlyList<string> next, IReadOnlyList<string> previous, IReadOnlyList<string> expected)
    {
      // Given
      // When
      var actual = BuildConfiguration.Combine(next, previous);

      // Then
      Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(DictionaryCombinations))]
    public void CombinesReadOnlyDictionaries(IReadOnlyDictionary<string, string> next, IReadOnlyDictionary<string, string> previous, IReadOnlyDictionary<string, string> expected)
    {
      // Given
      // When
      var actual = BuildConfiguration.Combine(next, previous);

      // Then
      Assert.Equal(expected, actual);
    }
  }
}
