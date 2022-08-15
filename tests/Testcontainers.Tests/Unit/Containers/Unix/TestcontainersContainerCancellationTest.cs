namespace DotNet.Testcontainers.Tests.Unit.Containers.Unix
{
  using System;
  using System.IO;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  public static class TestcontainersContainerCancellationTest
  {
    [Collection(nameof(Testcontainers))]
    public sealed class Cancel : IClassFixture<AlpineFixture>
    {
      private readonly AlpineFixture alpineFixture;

      public Cancel(AlpineFixture alpineFixture)
      {
        this.alpineFixture = alpineFixture;
      }

      [Fact]
      public async Task Start()
      {
        using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15)))
        {
          var expectedExceptions = new[] { typeof(TaskCanceledException), typeof(OperationCanceledException), typeof(TimeoutException), typeof(IOException) };

          // It depends which part in the StartAsync gets canceled. Catch base exception.
          var exception = await Assert.ThrowsAnyAsync<SystemException>(() => this.alpineFixture.Container.StartAsync(cts.Token));
          Assert.Contains(exception.GetType(), expectedExceptions);
        }
      }
    }
  }
}
