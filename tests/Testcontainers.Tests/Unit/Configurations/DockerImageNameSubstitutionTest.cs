namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Images;
  using Xunit;

  [CollectionDefinition(nameof(DockerImageNameSubstitutionTest), DisableParallelization = true)]
  public static class DockerImageNameSubstitutionTest
  {
    public abstract class ImageNameSubstitutionTest : IDisposable
    {
      private bool _disposed;

      protected ImageNameSubstitutionTest()
      {
        Reset();
      }

      public void Dispose()
      {
        Dispose(true);
        GC.SuppressFinalize(this);
      }

      protected virtual void Dispose(bool disposing)
      {
        if (_disposed)
        {
          return;
        }

        if (disposing)
        {
          Reset();
        }

        _disposed = true;
      }

      private static void Reset()
      {
        TestcontainersSettings.HubImageNamePrefix = string.Empty;
        TestcontainersSettings.ImageNameSubstitution = null;
      }
    }

    [Collection(nameof(DockerImageNameSubstitutionTest))]
    public sealed class ImageNameSubstitutionIsSet : ImageNameSubstitutionTest
    {
      [Fact]
      public void SubstitutesImageNameForStringConfiguration()
      {
        // Given
        TestcontainersSettings.ImageNameSubstitution = image => new DockerImage("my.proxy.com/mirror/" + image.FullName);

        // When
        IContainer container = new ContainerBuilder("bar:1.0.0")
          .Build();

        // Then
        Assert.Equal("my.proxy.com/mirror/bar:1.0.0", container.Image.FullName);
      }

      [Fact]
      public void SubstitutesImageNameForObjectConfiguration()
      {
        // Given
        TestcontainersSettings.ImageNameSubstitution = image => new DockerImage("my.proxy.com/mirror/" + image.FullName);

        IImage image = new DockerImage("bar:1.0.0");

        // When
        IContainer container = new ContainerBuilder(image)
          .Build();

        // Then
        Assert.Equal("my.proxy.com/mirror/bar:1.0.0", container.Image.FullName);
      }

      [Fact]
      public void SubstitutesImageNameBeforeHubImageNamePrefix()
      {
        // Given
        TestcontainersSettings.HubImageNamePrefix = "my.proxy.com";
        TestcontainersSettings.ImageNameSubstitution = image => new DockerImage("registry.azurecr.io/" + image.FullName);

        // When
        IContainer container = new ContainerBuilder("bar:1.0.0")
          .Build();

        // Then
        Assert.Equal("registry.azurecr.io/bar:1.0.0", container.Image.FullName);
      }

      [Fact]
      public void KeepsOriginalImageWhenSubstitutionReturnsNull()
      {
        // Given
        TestcontainersSettings.ImageNameSubstitution = _ => null;

        // When
        IContainer container = new ContainerBuilder("bar:1.0.0")
          .Build();

        // Then
        Assert.Equal("bar:1.0.0", container.Image.FullName);
      }
    }

    [Collection(nameof(DockerImageNameSubstitutionTest))]
    public sealed class HubImageNamePrefixIsSet : ImageNameSubstitutionTest
    {
      public static TheoryData<string, string, string> Substitutions { get; }
        = new TheoryData<string, string, string>
        {
          { "my.proxy.com", "bar", "my.proxy.com/bar:latest" },
          { "my.proxy.com", "bar:latest", "my.proxy.com/bar:latest" },
          { "my.proxy.com", "bar:1.0.0", "my.proxy.com/bar:1.0.0" },
          { "my.proxy.com/my-path", "bar:1.0.0", "my.proxy.com/my-path/bar:1.0.0" },
          { "my.proxy.com:443", "bar:1.0.0", "my.proxy.com:443/bar:1.0.0" },
          { "my.proxy.com", "foo/bar:1.0.0", "my.proxy.com/foo/bar:1.0.0" },
          { "my.proxy.com/my-path", "foo/bar:1.0.0", "my.proxy.com/my-path/foo/bar:1.0.0" },
          { "my.proxy.com:443", "foo/bar:1.0.0", "my.proxy.com:443/foo/bar:1.0.0" },
          { "my.proxy.com:443/my-path", "foo/bar:1.0.0", "my.proxy.com:443/my-path/foo/bar:1.0.0" },
          { "my.proxy.com", "myregistry.azurecr.io/foo/bar:1.0.0", "myregistry.azurecr.io/foo/bar:1.0.0" },
          { "my.proxy.com", "myregistry.azurecr.io:443/foo/bar:1.0.0", "myregistry.azurecr.io:443/foo/bar:1.0.0" },
        };

      [Theory]
      [MemberData(nameof(Substitutions))]
      public void PrependForStringConfiguration(string hubImageNamePrefix, string imageName, string expectedFullName)
      {
        // Given
        TestcontainersSettings.HubImageNamePrefix = hubImageNamePrefix;

        // When
        IContainer container = new ContainerBuilder(imageName)
          .Build();

        // Then
        Assert.Equal(expectedFullName, container.Image.FullName);
      }

      [Theory]
      [MemberData(nameof(Substitutions))]
      public void PrependForObjectConfiguration(string hubImageNamePrefix, string imageName, string expectedFullName)
      {
        // Given
        TestcontainersSettings.HubImageNamePrefix = hubImageNamePrefix;

        IImage image = new DockerImage(imageName);

        // When
        IContainer container = new ContainerBuilder(image)
          .Build();

        // Then
        Assert.Equal(expectedFullName, container.Image.FullName);
      }
    }

    [Collection(nameof(DockerImageNameSubstitutionTest))]
    public sealed class HubImageNamePrefixIsNotSet : ImageNameSubstitutionTest
    {
      [Fact]
      public void DoNotPrependForStringConfiguration()
      {
        // Given
        const string imageName = "bar:latest";

        // When
        IContainer container = new ContainerBuilder(imageName)
          .Build();

        // Then
        Assert.Equal(imageName, container.Image.FullName);
      }

      [Fact]
      public void DoNotPrependForObjectConfiguration()
      {
        // Given
        const string imageName = "bar:latest";

        IImage image = new DockerImage(imageName);

        // When
        IContainer container = new ContainerBuilder(image)
          .Build();

        // Then
        Assert.Equal(imageName, container.Image.FullName);
      }
    }
  }
}
