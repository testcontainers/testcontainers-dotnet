namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
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

    private readonly string _resourceMappingSourceFilePath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());

    private readonly string _resourceMappingFileDestinationFilePath = string.Join("/", string.Empty, "tmp", Path.GetRandomFileName());

    private readonly string _resourceMappingBytesDestinationFilePath = string.Join("/", string.Empty, "tmp", Path.GetRandomFileName());

    private readonly IContainer _container;

    public CopyResourceMappingContainerTest()
    {
      _container = new ContainerBuilder()
        .WithImage(CommonImages.Alpine)
        .WithResourceMapping(_resourceMappingSourceFilePath, _resourceMappingFileDestinationFilePath)
        .WithResourceMapping(Encoding.Default.GetBytes(ResourceMappingContent), _resourceMappingBytesDestinationFilePath)
        .Build();
    }

    public Task InitializeAsync()
    {
      File.WriteAllText(_resourceMappingSourceFilePath, ResourceMappingContent);
      return _container.StartAsync();
    }

    public Task DisposeAsync()
    {
      return _container.DisposeAsync().AsTask();
    }

    public void Dispose()
    {
      if (File.Exists(_resourceMappingSourceFilePath))
      {
        File.Delete(_resourceMappingSourceFilePath);
      }
    }

    [Fact]
    public async Task ReadExistingFile()
    {
      // Given
      var resourceMappingBytes = await Task.WhenAll(new[] { _resourceMappingFileDestinationFilePath, _resourceMappingBytesDestinationFilePath }
          .Select(resourceMappingFilePath => _container.ReadFileAsync(resourceMappingFilePath)))
        .ConfigureAwait(false);

      // When
      var resourceMappingContent = resourceMappingBytes.Select(Encoding.Default.GetString);

      // Then
      Assert.All(resourceMappingContent, item => Assert.Equal(ResourceMappingContent, item));
    }
  }
}
