namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.IO;
  using System.Text;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Commons;
  using DotNet.Testcontainers.Containers;
  using Xunit;

  public sealed class ReadFileFromContainerTest : IAsyncLifetime
  {
    private readonly IContainer _container = new ContainerBuilder()
      .WithImage("alpine")
      .WithEntrypoint(CommonCommands.SleepInfinity)
      .Build();

    [Fact]
    public async Task ReadExistingFile()
    {
      // Given
      const string dayOfWeekFilePath = "/tmp/dayOfWeek";

      var dayOfWeek = DateTime.UtcNow.DayOfWeek.ToString();

      // When
      _ = await _container.ExecAsync(new[] { "/bin/sh", "-c", $"echo {dayOfWeek} > {dayOfWeekFilePath}" }, TestContext.Current.CancellationToken)
        .ConfigureAwait(true);

      var fileContent = await _container.ReadFileAsync(dayOfWeekFilePath, TestContext.Current.CancellationToken)
        .ConfigureAwait(true);

      // Then
      Assert.Equal(dayOfWeek, Encoding.Default.GetString(fileContent).TrimEnd());
    }

    [Fact]
    public Task AccessNotExistingFileThrowsFileNotFoundException()
    {
      return Assert.ThrowsAsync<FileNotFoundException>(() => _container.ReadFileAsync("/tmp/fileNotFound", TestContext.Current.CancellationToken));
    }

    [Fact]
    public Task AccessDirectoryThrowsInvalidOperationException()
    {
      return Assert.ThrowsAsync<InvalidOperationException>(() => _container.ReadFileAsync("/tmp", TestContext.Current.CancellationToken));
    }

    public async ValueTask InitializeAsync()
    {
      await _container.StartAsync()
          .ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
      return _container.DisposeAsync();
    }
  }
}
