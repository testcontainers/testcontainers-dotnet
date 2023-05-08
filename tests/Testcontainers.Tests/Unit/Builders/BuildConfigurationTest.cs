namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Linq;
  using DotNet.Testcontainers.Builders;
  using Xunit;

  public sealed class BuildConfigurationTest
  {
    [Theory]
    [InlineData(null, null, null)]
    [InlineData(null, "B", "B")]
    [InlineData("A", null, "A")]
    [InlineData("A", "B", "B")]
    public void CombineReferenceTypes(string oldValue, string newValue, string expected)
    {
      var actual = BuildConfiguration.Combine(oldValue, newValue);
      Assert.Equal(expected, actual);
    }

    [Theory]
    [ClassData(typeof(EnumerableCombinationTestData))]
    public void CombineEnumerables(IEnumerable<string> oldValue, IEnumerable<string> newValue, IEnumerable<string> expected)
    {
      var actual = BuildConfiguration.Combine(oldValue, newValue);
      Assert.Equal(expected?.OrderBy(item => item), actual?.OrderBy(item => item));
    }

    [Theory]
    [ClassData(typeof(ReadOnlyListCombinationTestData))]
    public void CombineReadOnlyLists(IReadOnlyList<string> oldValue, IReadOnlyList<string> newValue, IReadOnlyList<string> expected)
    {
      var actual = BuildConfiguration.Combine(oldValue, newValue);
      Assert.Equal(expected, actual);
    }

    [Theory]
    [ClassData(typeof(DictionaryCombinationTestData))]
    public void CombineReadOnlyDictionaries(IReadOnlyDictionary<string, string> oldValue, IReadOnlyDictionary<string, string> newValue, IReadOnlyDictionary<string, string> expected)
    {
      var actual = BuildConfiguration.Combine(oldValue, newValue);
      Assert.Equal(expected, actual);
    }

    private sealed class EnumerableCombinationTestData : List<object[]>
    {
      public EnumerableCombinationTestData()
      {
        Add(new object[] { null, null, Array.Empty<string>() });
        Add(new object[] { null, new[] { "2" }, new[] { "2" } });
        Add(new object[] { new[] { "1" }, null, new[] { "1" } });
        Add(new object[] { new[] { "1" }, new[] { "2" }, new[] { "1", "2" } });
        Add(new object[] { new[] { "1", "2", "3" }, new[] { "2", "3", "4" }, new[] { "1", "2", "2", "3", "3", "4" } });
      }
    }

    private sealed class ReadOnlyListCombinationTestData : List<object[]>
    {
      public ReadOnlyListCombinationTestData()
      {
        Add(new object[] { null, null, Array.Empty<string>() });
        Add(new object[] { null, new[] { "2" }, new[] { "2" } });
        Add(new object[] { new[] { "1" }, null, new[] { "1" } });
        Add(new object[] { new[] { "1" }, new[] { "2" }, new[] { "1", "2" } });
        Add(new object[] { new[] { "1", "2", "3" }, new[] { "2", "3", "4" }, new[] { "1", "2", "3", "2", "3", "4" } });
      }
    }

    private sealed class DictionaryCombinationTestData : List<object[]>
    {
      public DictionaryCombinationTestData()
      {
        Add(new object[] { null, null, new ReadOnlyDictionary<string, string>(new Dictionary<string, string>()) });
        Add(new object[] { new Dictionary<string, string> { { "A", "A" } }, null, new Dictionary<string, string> { { "A", "A" } } });
        Add(new object[] { null, new Dictionary<string, string> { { "B", "B" } }, new Dictionary<string, string> { { "B", "B" } } });
        Add(new object[] { new Dictionary<string, string> { ["A"] = "old", ["B"] = "B" }, new Dictionary<string, string> { ["A"] = "new" }, new Dictionary<string, string> { ["A"] = "new", ["B"] = "B" } });
      }
    }
  }
}
