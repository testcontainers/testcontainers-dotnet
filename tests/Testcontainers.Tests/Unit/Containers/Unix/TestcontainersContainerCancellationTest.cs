namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.IO;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  public static class TestcontainersContainerCancellationTest
  {
    public sealed class Cancel : IClassFixture<AlpineFixture>
    {
      private readonly AlpineFixture _alpineFixture;

      public Cancel(AlpineFixture alpineFixture)
      {
        _alpineFixture = alpineFixture;
      }

      [Fact]
      public async Task Start()
      {
        // Given
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        var expectedExceptions = new[] { typeof(TaskCanceledException), typeof(OperationCanceledException), typeof(TimeoutException), typeof(IOException) };

        // When
        var exception = await Assert.ThrowsAnyAsync<SystemException>(() => _alpineFixture.Container.StartAsync(cts.Token))
          .ConfigureAwait(true);

        // Then
        Assert.Contains(exception.GetType(), expectedExceptions);
      }
    }
  }
}
