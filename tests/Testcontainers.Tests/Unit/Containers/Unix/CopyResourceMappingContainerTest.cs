namespace DotNet.Testcontainers.Tests.Unit.Containers.Unix
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Containers;
  using Xunit;

  public sealed class CopyResourceMappingContainerTest : IAsyncLifetime, IDisposable
  {
    private const string ResourceMappingContent = "👋";

    private readonly string resourceMappingSourceFilePath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());

    private readonly string resourceMappingFileDestinationFilePath = Path.Combine("/tmp", Path.GetTempFileName());

    private readonly string resourceMappingBytesDestinationFilePath = Path.Combine("/tmp", Path.GetTempFileName());

    private readonly IDockerContainer container;

    public CopyResourceMappingContainerTest()
    {
      this.container = new TestcontainersBuilder<TestcontainersContainer>()
        .WithImage("alpine")
        .WithResourceMapping(this.resourceMappingSourceFilePath, this.resourceMappingFileDestinationFilePath)
        .WithResourceMapping(Encoding.Default.GetBytes(ResourceMappingContent), this.resourceMappingBytesDestinationFilePath)
        .Build();
    }

    public Task InitializeAsync()
    {
      File.WriteAllText(this.resourceMappingSourceFilePath, ResourceMappingContent);
      return this.container.StartAsync();
    }

    public Task DisposeAsync()
    {
      return this.container.DisposeAsync().AsTask();
    }

    public void Dispose()
    {
      if (File.Exists(this.resourceMappingSourceFilePath))
      {
        File.Delete(this.resourceMappingSourceFilePath);
      }
    }

    [Fact]
    public async Task ReadExistingFile()
    {
      // Given
      var resourceMappingBytes = await Task.WhenAll(new[] { this.resourceMappingFileDestinationFilePath, this.resourceMappingBytesDestinationFilePath }
          .Select(resourceMappingFilePath => this.container.ReadFileAsync(resourceMappingFilePath)))
        .ConfigureAwait(false);

      // When
      var resourceMappingContent = resourceMappingBytes.Select(Encoding.Default.GetString);

      // Then
      Assert.All(resourceMappingContent, item => Assert.Equal(ResourceMappingContent, item));
    }
  }
}
