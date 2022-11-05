namespace DotNet.Testcontainers.Tests.Unit.Containers.Unix
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  public sealed class CopyResourceMappingContainerTest : IAsyncLifetime, IDisposable
  {
    private const string ResourceMappingContent = "👋";

    private readonly string filePath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());

    private readonly string fileResourceMappingFilePath = Path.Combine("/tmp", Path.GetTempFileName());

    private readonly string binaryResourceMappingFilePath = Path.Combine("/tmp", Path.GetTempFileName());

    private readonly IDockerContainer container;

    public CopyResourceMappingContainerTest()
    {
      this.container = new TestcontainersBuilder<TestcontainersContainer>()
        .WithImage("alpine")
        .WithEntrypoint(KeepTestcontainersUpAndRunning.Command)
        .WithResourceMapping(this.filePath, this.fileResourceMappingFilePath)
        .WithResourceMapping(Encoding.Default.GetBytes(ResourceMappingContent), this.binaryResourceMappingFilePath)
        .Build();
    }

    public Task InitializeAsync()
    {
      using (var fileStream = File.Create(this.filePath))
      {
        fileStream.Write(Encoding.Default.GetBytes(ResourceMappingContent));
        fileStream.Flush();
      }

      return this.container.StartAsync();
    }

    public async Task DisposeAsync()
    {
      await this.container.DisposeAsync();

      if (File.Exists(this.filePath))
      {
        File.Delete(this.filePath);
      }
    }

    public void Dispose()
    {
    }

    [Fact]
    public async Task ReadExistingFile()
    {
      // Given
      var resourceMappingBytes = await Task.WhenAll(new[] { this.fileResourceMappingFilePath, this.binaryResourceMappingFilePath }
          .Select(filePath => this.container.ReadFileAsync(filePath)))
        .ConfigureAwait(false);

      // When
      var resourceMappingContent = resourceMappingBytes.Select(Encoding.Default.GetString);

      // Then
      Assert.All(resourceMappingContent, item => Assert.Equal(ResourceMappingContent, item));
    }
  }
}
