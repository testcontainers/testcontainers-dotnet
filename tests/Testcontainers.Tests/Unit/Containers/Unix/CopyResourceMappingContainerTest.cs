namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Commons;
  using DotNet.Testcontainers.Containers;
  using Xunit;

  public sealed class CopyResourceMappingContainerTest : IAsyncLifetime, IDisposable
  {
    private const string ResourceMappingContent = "👋";

    private readonly FileInfo _testFile = new FileInfo(Path.Combine(TestSession.TempDirectoryPath, Path.GetRandomFileName()));

    private readonly string _bytesTargetFilePath;

    private readonly string _fileTargetFilePath;

    private readonly IContainer _container;

    public CopyResourceMappingContainerTest()
    {
      var resourceContent = Encoding.Default.GetBytes(ResourceMappingContent);

      using var fileStream = _testFile.Open(FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
      fileStream.Write(resourceContent);

      _bytesTargetFilePath = string.Join("/", string.Empty, "tmp", Guid.NewGuid(), _testFile.Name);

      _fileTargetFilePath = string.Join("/", string.Empty, "tmp", Guid.NewGuid());

      _container = new ContainerBuilder()
        .WithImage(CommonImages.Alpine)
        .WithResourceMapping(resourceContent, _bytesTargetFilePath)
        .WithResourceMapping(_testFile, _fileTargetFilePath)
        .Build();
    }

    public Task InitializeAsync()
    {
      return _container.StartAsync();
    }

    public Task DisposeAsync()
    {
      return _container.DisposeAsync().AsTask();
    }

    public void Dispose()
    {
      _testFile.Delete();
    }

    [Fact]
    public async Task ReadExistingFile()
    {
      // Given
      IList<string> targetFilePaths = new List<string>();
      targetFilePaths.Add(_bytesTargetFilePath);
      targetFilePaths.Add(string.Join("/", _fileTargetFilePath, _testFile.Name));

      // When
      var resourceContents = await Task.WhenAll(targetFilePaths.Select(containerFilePath => _container.ReadFileAsync(containerFilePath)))
        .ConfigureAwait(false);

      // Then
      Assert.All(resourceContents.Select(Encoding.Default.GetString), resourceContent => Assert.Equal(ResourceMappingContent, resourceContent));
    }
  }
}
