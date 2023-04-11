namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Collections.Generic;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Images;
  using Xunit;

  [CollectionDefinition(nameof(DockerImageNameSubstitutionTest), DisableParallelization = true)]
  public static class DockerImageNameSubstitutionTest
  {
    [Collection(nameof(DockerImageNameSubstitutionTest))]
    public sealed class HubImageNamePrefixIsSet : IDisposable
    {
      public static IEnumerable<object[]> Substitutions { get; }
        = new[]
        {
          new[] { "my.proxy.com", "bar", "my.proxy.com/bar:latest" },
          new[] { "my.proxy.com", "bar:latest", "my.proxy.com/bar:latest" },
          new[] { "my.proxy.com", "bar:1.0.0", "my.proxy.com/bar:1.0.0" },
          new[] { "my.proxy.com/my-path", "bar:1.0.0", "my.proxy.com/my-path/bar:1.0.0" },
          new[] { "my.proxy.com:443", "bar:1.0.0", "my.proxy.com:443/bar:1.0.0" },
          new[] { "my.proxy.com", "foo/bar:1.0.0", "my.proxy.com/foo/bar:1.0.0" },
          new[] { "my.proxy.com/my-path", "foo/bar:1.0.0", "my.proxy.com/my-path/foo/bar:1.0.0" },
          new[] { "my.proxy.com:443", "foo/bar:1.0.0", "my.proxy.com:443/foo/bar:1.0.0" },
          new[] { "my.proxy.com:443/my-path", "foo/bar:1.0.0", "my.proxy.com:443/my-path/foo/bar:1.0.0" },
          new[] { "my.proxy.com", "myregistry.azurecr.io/foo/bar:1.0.0", "myregistry.azurecr.io/foo/bar:1.0.0" },
          new[] { "my.proxy.com", "myregistry.azurecr.io:443/foo/bar:1.0.0", "myregistry.azurecr.io:443/foo/bar:1.0.0" },
        };

      [Theory]
      [MemberData(nameof(Substitutions))]
      public void PrependForStringConfiguration(string hubImageNamePrefix, string imageName, string expectedFullName)
      {
        // Given
        TestcontainersSettings.HubImageNamePrefix = hubImageNamePrefix;

        // When
        IContainer container = new ContainerBuilder()
          .WithImage(imageName)
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
        IContainer container = new ContainerBuilder()
          .WithImage(image)
          .Build();

        // Then
        Assert.Equal(expectedFullName, container.Image.FullName);
      }

      public void Dispose()
      {
        TestcontainersSettings.HubImageNamePrefix = string.Empty;
      }
    }

    [Collection(nameof(DockerImageNameSubstitutionTest))]
    public sealed class HubImageNamePrefixIsNotSet
    {
      public HubImageNamePrefixIsNotSet()
      {
        TestcontainersSettings.HubImageNamePrefix = string.Empty;
      }

      [Fact]
      public void DoNotPrependForStringConfiguration()
      {
        // Given
        const string imageName = "bar:latest";

        // When
        IContainer container = new ContainerBuilder()
          .WithImage(imageName)
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
        IContainer container = new ContainerBuilder()
          .WithImage(image)
          .Build();

        // Then
        Assert.Equal(imageName, container.Image.FullName);
      }
    }
  }
}
