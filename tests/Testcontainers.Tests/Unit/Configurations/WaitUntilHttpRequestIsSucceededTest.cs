namespace DotNet.Testcontainers.Tests.Unit.Configurations
{
  using System.Collections.Generic;
  using System.Net;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Commons;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using Microsoft.Extensions.Logging.Abstractions;
  using Xunit;

  public sealed class WaitUntilHttpRequestIsSucceededTest
  {
    public static IEnumerable<object[]> GetHttpWaitStrategies()
    {
      yield return new object[] { new HttpWaitStrategy() };
      yield return new object[] { new HttpWaitStrategy().ForStatusCode(HttpStatusCode.OK) };
      yield return new object[] { new HttpWaitStrategy().ForStatusCodeMatching(statusCode => HttpStatusCode.OK.Equals(statusCode)) };
      yield return new object[] { new HttpWaitStrategy().ForStatusCode(HttpStatusCode.Moved).ForStatusCodeMatching(statusCode => HttpStatusCode.OK.Equals(statusCode)) };
    }

    [Theory]
    [MemberData(nameof(GetHttpWaitStrategies))]
    public async Task HttpWaitStrategyShouldSucceeded(HttpWaitStrategy httpWaitStrategy)
    {
      // Given
      const ushort httpPort = 80;

      var container = new TestcontainersBuilder<TestcontainersContainer>()
        .WithImage(CommonImages.Nginx)
        .WithPortBinding(httpPort, true)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(httpPort))
        .Build();

      // When
      await container.StartAsync()
        .ConfigureAwait(false);

      var succeeded = await httpWaitStrategy.Until(container, NullLogger.Instance)
        .ConfigureAwait(false);

      // Then
      Assert.True(succeeded);
    }
  }
}
