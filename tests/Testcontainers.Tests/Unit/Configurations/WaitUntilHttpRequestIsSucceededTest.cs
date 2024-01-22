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

    public static TheoryData<HttpWaitStrategy> GetHttpWaitStrategies()
    {
      var theoryData = new TheoryData<HttpWaitStrategy>();
      theoryData.Add(new HttpWaitStrategy());
      theoryData.Add(new HttpWaitStrategy().ForPort(HttpPort));
      theoryData.Add(new HttpWaitStrategy().ForStatusCode(HttpStatusCode.OK));
      theoryData.Add(new HttpWaitStrategy().ForStatusCodeMatching(statusCode => HttpStatusCode.OK.Equals(statusCode)));
      theoryData.Add(new HttpWaitStrategy().ForResponseMessageMatching(response => Task.FromResult(response.IsSuccessStatusCode)));
      theoryData.Add(new HttpWaitStrategy().ForStatusCode(HttpStatusCode.MovedPermanently).ForStatusCodeMatching(statusCode => HttpStatusCode.OK.Equals(statusCode)));
      return theoryData;
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
        .ConfigureAwait(true);

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
        .ConfigureAwait(true);

      await Task.Delay(TimeSpan.FromSeconds(1))
        .ConfigureAwait(true);

      var (stdout, _) = await _container.GetLogsAsync()
        .ConfigureAwait(true);

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
        .ConfigureAwait(true);

      await Task.Delay(TimeSpan.FromSeconds(1))
        .ConfigureAwait(true);

      var (stdout, _) = await _container.GetLogsAsync()
        .ConfigureAwait(true);

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
        .ConfigureAwait(true);

      var exceptionOnSubsequentCall = await Record.ExceptionAsync(() => httpWaitStrategy.UntilAsync(_container))
        .ConfigureAwait(true);

      // Then
      Assert.Null(exceptionOnSubsequentCall);
    }
  }
}
