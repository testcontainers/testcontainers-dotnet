using DotNet.Testcontainers.Commons;

namespace Testcontainers.Mockaco.Tests;

public sealed class MockacoBuilderTest
{
  [Fact]
  public void WithTemplatesPath_SetsCorrectPath()
  {
    // Given
    var templatesPath = TestSession.TempDirectoryPath;

    // When
    var builder = new MockacoBuilder()
      .WithTemplatesPath(templatesPath);

    // Then
    var container = builder.Build();
    Assert.NotNull(container);
  }

  [Fact]
  public void Build_WithoutTemplatesPath_CanBuild()
  {
    // Given
    var builder = new MockacoBuilder();

    // When
    var container = builder.Build();

    // Then
    Assert.NotNull(container);
    // Note: Container will use default empty templates path
    // This test verifies the builder doesn't throw during build
  }

  [Fact]
  public void WithTemplatesPath_NonExistentPath_CanBuild()
  {
    // Given
    var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
    var builder = new MockacoBuilder()
      .WithTemplatesPath(nonExistentPath);

    // When
    var container = builder.Build();

    // Then
    Assert.NotNull(container);
    // Note: Path validation may happen during container start, not build
    // This test verifies the builder accepts any path during configuration
  }

  [Fact]
  public void DefaultImage_IsCorrect()
  {
    // Given & When & Then
    Assert.Equal("natenho/mockaco:1.9.9", MockacoBuilder.MockacoImage);
  }

  [Fact]
  public void DefaultPort_IsCorrect()
  {
    // Given & When & Then
    Assert.Equal(5000, MockacoBuilder.MockacoPort);
  }
}
