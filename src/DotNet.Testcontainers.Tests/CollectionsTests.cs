namespace DotNet.Testcontainers.Tests
{
  using System.Linq;
  using Xunit;

  using static DotNet.Testcontainers.Collections.Collections;

  public class CollectionsTests
  {
    [Fact]
    public void Test_ListOfImplementation_WithValidArgs_NoException()
    {
      // Given
      var items = new string[] { "Foo", "Bar", "Baz" };

      // When
      var listOf = ListOf(items);

      // Then
      Assert.Equal(items.Length, listOf.Intersect(items).Count());
    }
  }
}
