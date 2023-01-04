namespace DotNet.Testcontainers.Tests.Unit.Configurations
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Net;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Commons;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using Microsoft.Extensions.Logging.Abstractions;
  using Xunit;

  public sealed class WaitUntilHttpRequestIsSucceededTest : IAsyncLifetime
  {
    private const ushort HttpPort = 80;

    private readonly ITestcontainersContainer container = new TestcontainersBuilder<TestcontainersContainer>()
      .WithImage(CommonImages.Alpine)
      .WithEntrypoint("/bin/sh", "-c")
      .WithCommand($"echo \"HTTP/1.1 200 OK\r\n\" | nc -l -p {HttpPort}")
      .WithPortBinding(HttpPort, true)
      .Build();

    public static IEnumerable<object[]> GetHttpWaitStrategies()
    {
      yield return new object[] { new HttpWaitStrategy() };
      yield return new object[] { new HttpWaitStrategy().ForPort(HttpPort) };
      yield return new object[] { new HttpWaitStrategy().ForStatusCode(HttpStatusCode.OK) };
      yield return new object[] { new HttpWaitStrategy().ForStatusCodeMatching(statusCode => HttpStatusCode.OK.Equals(statusCode)) };
      yield return new object[] { new HttpWaitStrategy().ForStatusCode(HttpStatusCode.MovedPermanently).ForStatusCodeMatching(statusCode => HttpStatusCode.OK.Equals(statusCode)) };
    }

    public Task InitializeAsync()
    {
      return this.container.StartAsync();
    }

    public Task DisposeAsync()
    {
      return this.container.DisposeAsync().AsTask();
    }

    [Theory]
    [MemberData(nameof(GetHttpWaitStrategies))]
    public async Task HttpWaitStrategyReceivesStatusCode(HttpWaitStrategy httpWaitStrategy)
    {
      var succeeded = await httpWaitStrategy.Until(this.container, NullLogger.Instance)
        .ConfigureAwait(false);

      Assert.True(succeeded);
    }

    [Fact]
    public async Task HttpWaitStrategySendsHeaders()
    {
      // Given
      var httpHeaders = new Dictionary<string, string> { { "Authorization", "Basic QWxhZGRpbjpvcGVuIHNlc2FtZQ==" } };

      var httpWaitStrategy = new HttpWaitStrategy().WithHeaders(httpHeaders);

      // When
      var succeeded = await httpWaitStrategy.Until(this.container, NullLogger.Instance)
        .ConfigureAwait(false);

      await Task.Delay(TimeSpan.FromSeconds(1))
        .ConfigureAwait(false);

      var (stdout, _) = await this.container.GetLogs()
        .ConfigureAwait(false);

      // Then
      Assert.True(succeeded);
      Assert.Contains(httpHeaders.First().Key, stdout);
      Assert.Contains(httpHeaders.First().Value, stdout);
    }
  }
}
