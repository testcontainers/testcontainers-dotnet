namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using Xunit;

  public sealed class BuildConfigurationTest
  {
    [Theory]
    [InlineData(null, null, null)]
    [InlineData(null, "B", "B")]
    [InlineData("A", null, "A")]
    [InlineData("A", "B", "B")]
    public void CombineReferenceTypes(
      string oldValue,
      string newValue,
      string expected)
    {
      var actual = BuildConfiguration.Combine(oldValue, newValue);
      Assert.Equal(expected, actual);
    }

    [Theory]
    [ClassData(typeof(EnumerableCombinationTestData))]
    public void CombineEnumerables(
      IEnumerable<string> oldValue,
      IEnumerable<string> newValue,
      IEnumerable<string> expected)
    {
      var actual = BuildConfiguration.Combine(oldValue, newValue);
      Assert.Equal(expected?.OrderBy(item => item), actual?.OrderBy(item => item));
    }

    [Theory]
    [ClassData(typeof(ReadOnlyListCombinationTestData))]
    public void CombineReadOnlyLists(
      IReadOnlyList<string> oldValue,
      IReadOnlyList<string> newValue,
      IReadOnlyList<string> expected)
    {
      var actual = BuildConfiguration.Combine(oldValue, newValue);
      Assert.Equal(expected, actual);
    }

    [Theory]
    [ClassData(typeof(DictionaryCombinationTestData))]
    public void CombineReadOnlyDictionaries(
      IReadOnlyDictionary<string, string> oldValue,
      IReadOnlyDictionary<string, string> newValue,
      IReadOnlyDictionary<string, string> expected)
    {
      var actual = BuildConfiguration.Combine(oldValue, newValue);
      Assert.Equal(expected, actual);
    }

    [Theory]
    [ClassData(typeof(ComposableEnumerableCombinationTestData))]
    public void CombineComposableEnumerables(
      ComposableEnumerable<string> oldValue,
      ComposableEnumerable<string> newValue,
      IEnumerable<string> expected)
    {
      var actual = BuildConfiguration.Combine(oldValue, newValue);
      Assert.Equal(expected, actual);
    }

    [Theory]
    [ClassData(typeof(AppendEnumerableTestData))]
    public void AppendEnumerableCompose(
      IEnumerable<string> oldValue,
      IEnumerable<string> newValue,
      IEnumerable<string> expected)
    {
      var append = new AppendEnumerable<string>(newValue);
      var result = append.Compose(oldValue);
      Assert.Equal(expected, result);
    }

    [Theory]
    [ClassData(typeof(OverwriteEnumerableTestData))]
    public void OverwriteEnumerableCompose(
      IEnumerable<string> oldValue,
      IEnumerable<string> newValue,
      IEnumerable<string> expected)
    {
      var overwrite = new OverwriteEnumerable<string>(newValue);
      var result = overwrite.Compose(oldValue);
      Assert.Equal(expected, result);
    }

    [Theory]
    [ClassData(typeof(AppendDictionaryTestData))]
    public void AppendDictionaryCompose(
      IReadOnlyDictionary<string, string> oldValue,
      IReadOnlyDictionary<string, string> newValue,
      IReadOnlyDictionary<string, string> expected)
    {
      var append = new AppendDictionary<string, string>(newValue);
      var result = append.Compose(oldValue);
      Assert.Equal(expected, result);
    }

    [Theory]
    [ClassData(typeof(OverwriteDictionaryTestData))]
    public void OverwriteDictionaryCompose(
      IReadOnlyDictionary<string, string> oldValue,
      IReadOnlyDictionary<string, string> newValue,
      IReadOnlyDictionary<string, string> expected)
    {
      var overwrite = new OverwriteDictionary<string, string>(newValue);
      var result = overwrite.Compose(oldValue);
      Assert.Equal(expected, result);
    }

    [Fact]
    public void ComposableEnumerableHandlesNullCollection()
    {
      var append = new AppendEnumerable<string>(null);
      Assert.Empty(append);

      var overwrite = new OverwriteEnumerable<string>(null);
      Assert.Empty(overwrite);
    }

    [Fact]
    public void ComposableDictionaryHandlesNullDictionary()
    {
      var append = new AppendDictionary<string, string>(null);
      Assert.Empty(append);

      var overwrite = new OverwriteDictionary<string, string>(null);
      Assert.Empty(overwrite);
    }

    private sealed class EnumerableCombinationTestData
      : TheoryData<
          IEnumerable<string>,
          IEnumerable<string>,
          IEnumerable<string>
        >
    {
      public EnumerableCombinationTestData()
      {
        Add(null,
            null,
            Array.Empty<string>());

        Add(null,
            new[] { "2" },
            new[] { "2" });

        Add(new[] { "1" },
            null,
            new[] { "1" });

        Add(new[] { "1" },
            new[] { "2" },
            new[] { "1", "2" });

        Add(new[] { "1", "2", "3" },
            new[] { "2", "3", "4" },
            new[] { "1", "2", "2", "3", "3", "4" });
      }
    }

    private sealed class ReadOnlyListCombinationTestData
      : TheoryData<
          IReadOnlyList<string>,
          IReadOnlyList<string>,
          IReadOnlyList<string>
        >
    {
      public ReadOnlyListCombinationTestData()
      {
        Add(null,
            null,
            Array.Empty<string>());

        Add(null,
            new[] { "2" },
            new[] { "2" });

        Add(new[] { "1" },
            null,
            new[] { "1" });

        Add(new[] { "1" },
            new[] { "2" },
            new[] { "1", "2" });

        Add(new[] { "1", "2", "3" },
            new[] { "2", "3", "4" },
            new[] { "1", "2", "3", "2", "3", "4" });
      }
    }

    private sealed class DictionaryCombinationTestData
      : TheoryData<
          IReadOnlyDictionary<string, string>,
          IReadOnlyDictionary<string, string>,
          IReadOnlyDictionary<string, string>
        >
    {
      public DictionaryCombinationTestData()
      {
        Add(null,
            null,
            new Dictionary<string, string>());

        Add(new Dictionary<string, string> { ["A"] = "A" },
            null,
            new Dictionary<string, string> { ["A"] = "A" });

        Add(null,
            new Dictionary<string, string> { ["B"] = "B" },
            new Dictionary<string, string> { ["B"] = "B" });

        Add(new Dictionary<string, string> { ["A"] = "old", ["B"] = "B" },
            new Dictionary<string, string> { ["A"] = "new" },
            new Dictionary<string, string> { ["A"] = "new", ["B"] = "B" });
      }
    }

    private sealed class ComposableEnumerableCombinationTestData
      : TheoryData<
          ComposableEnumerable<string>,
          ComposableEnumerable<string>,
          IEnumerable<string>
        >
    {
      public ComposableEnumerableCombinationTestData()
      {
        Add(null,
            null,
            Array.Empty<string>());

        Add(null,
            new AppendEnumerable<string>(new[] { "2" }),
            new[] { "2" });

        Add(new AppendEnumerable<string>(new[] { "1" }),
            null,
            new[] { "1" });

        Add(new AppendEnumerable<string>(new[] { "1" }),
            new AppendEnumerable<string>(new[] { "2" }),
            new[] { "1", "2" });

        Add(new AppendEnumerable<string>(new[] { "1", "2" }),
            new OverwriteEnumerable<string>(new[] { "3", "4" }),
            new[] { "3", "4" });

        Add(new AppendEnumerable<string>(new[] { "1", "2", "3" }),
            new AppendEnumerable<string>(new[] { "4", "5" }),
            new[] { "1", "2", "3", "4", "5" });
      }
    }

    private sealed class AppendEnumerableTestData
      : TheoryData<
          IEnumerable<string>,
          IEnumerable<string>,
          IEnumerable<string>
        >
    {
      public AppendEnumerableTestData()
      {
        Add(Array.Empty<string>(),
            Array.Empty<string>(),
            Array.Empty<string>());

        Add(new[] { "old" },
            Array.Empty<string>(),
            new[] { "old" });

        Add(Array.Empty<string>(),
            new[] { "new" },
            new[] { "new" });

        Add(new[] { "old" },
            new[] { "new" },
            new[] { "old", "new" });

        Add(new[] { "1", "2" },
            new[] { "3", "4" },
            new[] { "1", "2", "3", "4" });

        Add(new[] { "A", "B", "C" },
            new[] { "X", "Y" },
            new[] { "A", "B", "C", "X", "Y" });
      }
    }

    private sealed class OverwriteEnumerableTestData
      : TheoryData<
          IEnumerable<string>,
          IEnumerable<string>,
          IEnumerable<string>
        >
    {
      public OverwriteEnumerableTestData()
      {
        Add(Array.Empty<string>(),
            Array.Empty<string>(),
            Array.Empty<string>());

        Add(new[] { "old" },
            Array.Empty<string>(),
            Array.Empty<string>());

        Add(Array.Empty<string>(),
            new[] { "new" },
            new[] { "new" });

        Add(new[] { "old" },
            new[] { "new" },
            new[] { "new" });

        Add(new[] { "1", "2" },
            new[] { "3", "4" },
            new[] { "3", "4" });

        Add(new[] { "A", "B", "C" },
            new[] { "X", "Y", "Z" },
            new[] { "X", "Y", "Z" });
      }
    }

    private sealed class AppendDictionaryTestData
      : TheoryData<
          IReadOnlyDictionary<string, string>,
          IReadOnlyDictionary<string, string>,
          IReadOnlyDictionary<string, string>
        >
    {
      public AppendDictionaryTestData()
      {
        Add(new Dictionary<string, string>(),
            new Dictionary<string, string>(),
            new Dictionary<string, string>());

        Add(new Dictionary<string, string> { ["A"] = "old" },
            new Dictionary<string, string>(),
            new Dictionary<string, string> { ["A"] = "old" });

        Add(new Dictionary<string, string>(),
            new Dictionary<string, string> { ["B"] = "new" },
            new Dictionary<string, string> { ["B"] = "new" });

        Add(new Dictionary<string, string> { ["A"] = "old" },
            new Dictionary<string, string> { ["B"] = "new" },
            new Dictionary<string, string> { ["A"] = "old", ["B"] = "new" });

        Add(new Dictionary<string, string> { ["A"] = "old", ["B"] = "keep" },
            new Dictionary<string, string> { ["A"] = "new" },
            new Dictionary<string, string> { ["A"] = "new", ["B"] = "keep" });
      }
    }

    private sealed class OverwriteDictionaryTestData
      : TheoryData<
          IReadOnlyDictionary<string, string>,
          IReadOnlyDictionary<string, string>,
          IReadOnlyDictionary<string, string>
        >
    {
      public OverwriteDictionaryTestData()
      {
        Add(new Dictionary<string, string>(),
            new Dictionary<string, string>(),
            new Dictionary<string, string>());

        Add(new Dictionary<string, string> { ["A"] = "old" },
            new Dictionary<string, string>(),
            new Dictionary<string, string>());

        Add(new Dictionary<string, string>(),
            new Dictionary<string, string> { ["B"] = "new" },
            new Dictionary<string, string> { ["B"] = "new" });

        Add(new Dictionary<string, string> { ["A"] = "old" },
            new Dictionary<string, string> { ["B"] = "new" },
            new Dictionary<string, string> { ["B"] = "new" });

        Add(new Dictionary<string, string> { ["A"] = "old", ["B"] = "ignore" },
            new Dictionary<string, string> { ["A"] = "new", ["C"] = "keep" },
            new Dictionary<string, string> { ["A"] = "new", ["C"] = "keep" });
      }
    }
  }
}
