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
  using Xunit;

  public sealed class WaitUntilHttpRequestIsSucceededTest : IAsyncLifetime
  {
    private const ushort HttpPort = 80;

    private readonly IContainer container = new TestcontainersBuilder<TestcontainersContainer>()
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
      yield return new object[] { new HttpWaitStrategy().ForResponseMessageMatching(response => Task.FromResult(response.IsSuccessStatusCode)) };
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
      var succeeded = await httpWaitStrategy.UntilAsync(this.container)
        .ConfigureAwait(false);

      Assert.True(succeeded);
    }

    [Fact]
    public async Task HttpWaitStrategySendsHeaders()
    {
      // Given
      const string username = "Aladdin";

      const string password = "open sesame";

      var httpHeaders = new Dictionary<string, string> { { "Connection", "keep-alive" } };

      var httpWaitStrategy = new HttpWaitStrategy().WithBasicAuthentication(username, password).WithHeaders(httpHeaders);

      // When
      var succeeded = await httpWaitStrategy.UntilAsync(this.container)
        .ConfigureAwait(false);

      await Task.Delay(TimeSpan.FromSeconds(1))
        .ConfigureAwait(false);

      var (stdout, _) = await this.container.GetLogs()
        .ConfigureAwait(false);

      // Then
      Assert.True(succeeded);
      Assert.Contains("Authorization", stdout);
      Assert.Contains("QWxhZGRpbjpvcGVuIHNlc2FtZQ==", stdout);
      Assert.Contains(httpHeaders.First().Key, stdout);
      Assert.Contains(httpHeaders.First().Value, stdout);
    }
  }
}
