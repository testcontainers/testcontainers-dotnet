namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Net;
  using System.Net.Http;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Commons;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using Xunit;

  public sealed class WaitUntilHttpRequestIsSucceededTest : IAsyncLifetime
  {
    private const ushort HttpPort = 80;

    private readonly IContainer _container = new ContainerBuilder()
      .WithImage(CommonImages.Alpine)
      .WithEntrypoint("/bin/sh", "-c")
      .WithCommand($"while true; do echo \"HTTP/1.1 200 OK\r\n\" | nc -l -p {HttpPort}; done")
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
      return _container.StartAsync();
    }

    public Task DisposeAsync()
    {
      return _container.DisposeAsync().AsTask();
    }

    [Theory]
    [MemberData(nameof(GetHttpWaitStrategies))]
    public async Task HttpWaitStrategyReceivesStatusCode(HttpWaitStrategy httpWaitStrategy)
    {
      var succeeded = await httpWaitStrategy.UntilAsync(_container)
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
      var succeeded = await httpWaitStrategy.UntilAsync(_container)
        .ConfigureAwait(false);

      await Task.Delay(TimeSpan.FromSeconds(1))
        .ConfigureAwait(false);

      var (stdout, _) = await _container.GetLogsAsync()
        .ConfigureAwait(false);

      // Then
      Assert.True(succeeded);
      Assert.Contains("Authorization", stdout);
      Assert.Contains("QWxhZGRpbjpvcGVuIHNlc2FtZQ==", stdout);
      Assert.Contains(httpHeaders.First().Key, stdout);
      Assert.Contains(httpHeaders.First().Value, stdout);
    }

    [Fact]
    public async Task HttpWaitStrategyUsesCustomHttpClientHandler()
    {
      // Given
      var cookieContainer = new CookieContainer();
      cookieContainer.Add(new Cookie("Key1", "Value1", "/", _container.Hostname));

      using var httpMessageHandler = new HttpClientHandler();
      httpMessageHandler.CookieContainer = cookieContainer;

      var httpWaitStrategy = new HttpWaitStrategy().UsingHttpMessageHandler(httpMessageHandler);

      // When
      var succeeded = await httpWaitStrategy.UntilAsync(_container)
        .ConfigureAwait(false);

      await Task.Delay(TimeSpan.FromSeconds(1))
        .ConfigureAwait(false);

      var (stdout, _) = await _container.GetLogsAsync()
        .ConfigureAwait(false);

      // Then
      Assert.True(succeeded);
      Assert.Contains("Cookie", stdout);
      Assert.Contains("Key1=Value1", stdout);
    }

    [Fact]
    public async Task HttpWaitStrategyReusesCustomHttpClientHandler()
    {
      // Given
      using var httpMessageHandler = new HttpClientHandler();

      var httpWaitStrategy = new HttpWaitStrategy().UsingHttpMessageHandler(httpMessageHandler);

      // When
      await httpWaitStrategy.UntilAsync(_container)
        .ConfigureAwait(false);

      var exceptionOnSubsequentCall = await Record.ExceptionAsync(() => httpWaitStrategy.UntilAsync(_container))
        .ConfigureAwait(false);

      // Then
      Assert.Null(exceptionOnSubsequentCall);
    }
  }
}
