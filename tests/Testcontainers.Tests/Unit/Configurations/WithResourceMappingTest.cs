using System;
using System.IO;

namespace DotNet.Testcontainers.Tests.Unit
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  public sealed class WithResourceMappingTest(AlpineBuilderFixture image) : IClassFixture<AlpineBuilderFixture>
  {
    [Fact]
    public async Task WithResourceMappingUriDirectoryInfo()
    {
      // Given
      var hostDir = new DirectoryInfo(Path.GetTempPath()).CreateSubdirectory("test");
      var hostFile = new FileInfo(Path.Combine(hostDir.FullName, "test.txt"));

      var expectedContent = new byte[20];
      Random.Shared.NextBytes(expectedContent);
      using (var fileStream = hostFile.OpenWrite())
      {
        fileStream.Write(expectedContent, 0, expectedContent.Length);
      }

      // When
      var container = image.Container(b =>
        b.WithResourceMapping(new Uri($"file://{hostFile.FullName}"), new DirectoryInfo("/tmp/")));
      await container.StartAsync(TestContext.Current.CancellationToken)
        .ConfigureAwait(true);

      // Then
      var actualContent = await container.ReadFileAsync($"/tmp/{hostFile.Name}", TestContext.Current.CancellationToken)
        .ConfigureAwait(true);
      Assert.Equal(expectedContent, actualContent);
    }

    [Fact]
    public async Task WithResourceMappingUriFileInfo()
    {
      // Given
      var hostDir = new DirectoryInfo(Path.GetTempPath()).CreateSubdirectory("test");
      var hostFile = new FileInfo(Path.Combine(hostDir.FullName, "test.txt"));

      var expectedContent = new byte[20];
      Random.Shared.NextBytes(expectedContent);
      using (var fileStream = hostFile.OpenWrite())
      {
        fileStream.Write(expectedContent, 0, expectedContent.Length);
      }

      // When
      var container = image.Container(b =>
        b.WithResourceMapping(new Uri($"file://{hostFile.FullName}"), new FileInfo("/tmp/test.txt")));
      await container.StartAsync(TestContext.Current.CancellationToken)
        .ConfigureAwait(true);

      // Then
      var actualContent = await container.ReadFileAsync($"/tmp/test.txt", TestContext.Current.CancellationToken)
        .ConfigureAwait(true);
      Assert.Equal(expectedContent, actualContent);
    }

    [Fact]
    public async Task WithResourceMappingUriString()
    {
      // Given
      var hostDir = new DirectoryInfo(Path.GetTempPath()).CreateSubdirectory("test");
      var hostFile = new FileInfo(Path.Combine(hostDir.FullName, "test.txt"));

      var expectedContent = new byte[20];
      Random.Shared.NextBytes(expectedContent);
      using (var fileStream = hostFile.OpenWrite())
      {
        fileStream.Write(expectedContent, 0, expectedContent.Length);
      }

      // When
      var container = image.Container(b =>
        b.WithResourceMapping(new Uri($"file://{hostFile.FullName}"), "/tmp/"));
      await container.StartAsync(TestContext.Current.CancellationToken)
        .ConfigureAwait(true);

      // Then
      var actualContent = await container.ReadFileAsync($"/tmp/{hostFile.Name}", TestContext.Current.CancellationToken)
        .ConfigureAwait(true);
      Assert.Equal(expectedContent, actualContent);
    }

    [Fact]
    public async Task WithResourceMappingStringString()
    {
      // Given
      var hostDir = new DirectoryInfo(Path.GetTempPath()).CreateSubdirectory("test");
      var hostFile = new FileInfo(Path.Combine(hostDir.FullName, "test.txt"));

      var expectedContent = new byte[20];
      Random.Shared.NextBytes(expectedContent);
      using (var fileStream = hostFile.OpenWrite())
      {
        fileStream.Write(expectedContent, 0, expectedContent.Length);
      }

      // When
      var container = image.Container(b =>
        b.WithResourceMapping(hostDir.FullName, "/tmp"));
      await container.StartAsync(TestContext.Current.CancellationToken)
        .ConfigureAwait(true);

      // Then
      var actualContent = await container.ReadFileAsync($"/tmp/{hostFile.Name}", TestContext.Current.CancellationToken)
        .ConfigureAwait(true);
      Assert.Equal(expectedContent, actualContent);
    }

    [Fact]
    public async Task WithResourceMappingDirectoryInfoDirectoryInfo()
    {
      // Given
      var hostDir = new DirectoryInfo(Path.GetTempPath()).CreateSubdirectory("test");
      var hostFile = new FileInfo(Path.Combine(hostDir.FullName, "test.txt"));

      var expectedContent = new byte[20];
      Random.Shared.NextBytes(expectedContent);
      using (var fileStream = hostFile.OpenWrite())
      {
        fileStream.Write(expectedContent, 0, expectedContent.Length);
      }

      // When
      var container = image.Container(b =>
        b.WithResourceMapping(hostDir, new DirectoryInfo("/tmp/")));
      await container.StartAsync(TestContext.Current.CancellationToken)
        .ConfigureAwait(true);

      // Then
      var actualContent = await container.ReadFileAsync($"/tmp/{hostFile.Name}", TestContext.Current.CancellationToken)
        .ConfigureAwait(true);
      Assert.Equal(expectedContent, actualContent);
    }

    [Fact]
    public async Task WithResourceMappingDirectoryInfoString()
    {
      // Given
      var hostDir = new DirectoryInfo(Path.GetTempPath()).CreateSubdirectory("test");
      var hostFile = new FileInfo(Path.Combine(hostDir.FullName, "test.txt"));

      var expectedContent = new byte[20];
      Random.Shared.NextBytes(expectedContent);
      using (var fileStream = hostFile.OpenWrite())
      {
        fileStream.Write(expectedContent, 0, expectedContent.Length);
      }

      // When
      var container = image.Container(b =>
        b.WithResourceMapping(hostDir, "/tmp/"));
      await container.StartAsync(TestContext.Current.CancellationToken)
        .ConfigureAwait(true);

      // Then
      var actualContent = await container.ReadFileAsync($"/tmp/{hostFile.Name}", TestContext.Current.CancellationToken)
        .ConfigureAwait(true);
      Assert.Equal(expectedContent, actualContent);
    }

    [Fact]
    public async Task WithResourceMappingFileInfoFileInfo()
    {
      // Given
      var hostFile = new FileInfo(Path.GetTempFileName());
      var expectedContent = new byte[20];
      Random.Shared.NextBytes(expectedContent);
      using (var fileStream = hostFile.OpenWrite())
      {
        fileStream.Write(expectedContent, 0, expectedContent.Length);
      }

      // When
      var container = image.Container(b =>
        b.WithResourceMapping(hostFile, new FileInfo("/tmp/test.txt")));
      await container.StartAsync(TestContext.Current.CancellationToken)
        .ConfigureAwait(true);

      // Then
      var actualContent = await container.ReadFileAsync("/tmp/test.txt", TestContext.Current.CancellationToken)
        .ConfigureAwait(true);
      Assert.Equal(expectedContent, actualContent);
    }

    [Fact]
    public async Task WithResourceMappingFileInfoDirectoryInfo()
    {
      // Given
      var hostFile = new FileInfo(Path.GetTempFileName());
      var expectedContent = new byte[20];
      Random.Shared.NextBytes(expectedContent);
      using (var fileStream = hostFile.OpenWrite())
      {
        fileStream.Write(expectedContent, 0, expectedContent.Length);
      }

      // When
      var container = image.Container(b =>
        b.WithResourceMapping(hostFile, new DirectoryInfo("/tmp")));
      await container.StartAsync(TestContext.Current.CancellationToken)
        .ConfigureAwait(true);

      // Then
      var actualContent = await container.ReadFileAsync($"/tmp/{hostFile.Name}", TestContext.Current.CancellationToken)
        .ConfigureAwait(true);
      Assert.Equal(expectedContent, actualContent);
    }

    [Fact]
    public async Task WithResourceMappingFileInfoString()
    {
      // Given
      var hostFile = new FileInfo(Path.GetTempFileName());
      var expectedContent = new byte[20];
      Random.Shared.NextBytes(expectedContent);
      using (var fileStream = hostFile.OpenWrite())
      {
        fileStream.Write(expectedContent, 0, expectedContent.Length);
      }

      // When
      var container = image.Container(b =>
        b.WithResourceMapping(hostFile, "/tmp/"));
      await container.StartAsync(TestContext.Current.CancellationToken)
        .ConfigureAwait(true);

      // Then
      var actualContent = await container.ReadFileAsync($"/tmp/{hostFile.Name}", TestContext.Current.CancellationToken)
        .ConfigureAwait(true);
      Assert.Equal(expectedContent, actualContent);
    }

    [Fact]
    public async Task WithResourceMappingBytesString()
    {
      // Given
      var expectedContent = new byte[100];
      Random.Shared.NextBytes(expectedContent);

      // When
      var container = image.Container(b =>
        b.WithResourceMapping(expectedContent, "/tmp/test.txt"));
      await container.StartAsync(TestContext.Current.CancellationToken)
        .ConfigureAwait(true);

      // Then
      var actualContent = await container.ReadFileAsync("/tmp/test.txt", TestContext.Current.CancellationToken)
        .ConfigureAwait(true);
      Assert.Equal(expectedContent, actualContent);
    }

    [Fact]
    public async Task WithResourceMappingBytesFileInfo()
    {
      // Given
      var expectedContent = new byte[100];
      Random.Shared.NextBytes(expectedContent);

      // When
      var container = image.Container(b =>
        b.WithResourceMapping(expectedContent, new FileInfo("/test.txt")));
      await container.StartAsync(TestContext.Current.CancellationToken)
        .ConfigureAwait(true);

      // Then
      var actualContent = await container.ReadFileAsync("/test.txt", TestContext.Current.CancellationToken)
        .ConfigureAwait(true);
      Assert.Equal(expectedContent, actualContent);
    }
  }
}
