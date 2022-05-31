namespace DotNet.Testcontainers.Tests.Unit.Containers.Unix
{
  using System;
  using System.IO;
  using System.Text;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  public sealed class ReadFileFromContainerTest : IAsyncLifetime
  {
    private readonly IDockerContainer container = new TestcontainersBuilder<TestcontainersContainer>()
      .WithImage("alpine")
      .WithEntrypoint(KeepTestcontainersUpAndRunning.Command)
      .Build();

    [Fact]
    public async Task ReadExistingFile()
    {
      // Given
      const string dayOfWeekFilePath = "/tmp/dayOfWeek";

      var dayOfWeek = DateTime.UtcNow.DayOfWeek.ToString();

      // When
      _ = await this.container.ExecAsync(new[] { "/bin/sh", "-c", $"echo {dayOfWeek} > {dayOfWeekFilePath}" })
        .ConfigureAwait(false);

      var fileContent = await this.container.ReadFileAsync(dayOfWeekFilePath)
        .ConfigureAwait(false);

      // Then
      Assert.Equal(dayOfWeek, Encoding.ASCII.GetString(fileContent).TrimEnd());
    }

    [Fact]
    public Task AccessNotExistingFileThrowsFileNotFoundException()
    {
      return Assert.ThrowsAsync<FileNotFoundException>(() => this.container.ReadFileAsync("/tmp/fileNotFound"));
    }

    [Fact]
    public Task AccessDirectoryThrowsInvalidOperationException()
    {
      return Assert.ThrowsAsync<InvalidOperationException>(() => this.container.ReadFileAsync("/tmp"));
    }

    public Task InitializeAsync()
    {
      return this.container.StartAsync();
    }

    public Task DisposeAsync()
    {
      return this.container.DisposeAsync().AsTask();
    }
  }
}
