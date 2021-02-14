namespace DotNet.Testcontainers.Tests.Unit.Containers.Unix
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Tests.Fixtures.Containers.Modules;
  using Xunit;

  public static class TestcontainersContainerCancellationTest
  {
    public class Cancel : IClassFixture<AlpineFixture>
    {
      private readonly AlpineFixture alpineFixture;

      public Cancel(AlpineFixture alpineFixture)
      {
        this.alpineFixture = alpineFixture;
      }

      [Fact]
      public async Task Start()
      {
        using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(0)))
        {
          var expectedExceptions = new[] { typeof(OperationCanceledException), typeof(TimeoutException) };

          // It depends on which part in the StartAsync gets canceled. Catch base exception.
          var exception = await Assert.ThrowsAnyAsync<SystemException>(() => this.alpineFixture.Container.StartAsync(cts.Token));
          Assert.Contains(exception.GetType(), expectedExceptions);
        }
      }
    }
  }
}
