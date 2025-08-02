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
  using JetBrains.Annotations;
  using Xunit;

  public sealed class WaitUntilHttpRequestIsSucceededTest : IClassFixture<WaitUntilHttpRequestIsSucceededTest.HttpFixture>
  {
    private const ushort HttpPort = 80;

    private readonly IContainer _container;

    public WaitUntilHttpRequestIsSucceededTest(HttpFixture httpFixture)
    {
      _container = httpFixture.Container;
    }

    public static TheoryData<HttpWaitStrategy> HttpWaitStrategies { get; }
      = new TheoryData<HttpWaitStrategy>
      {
        new HttpWaitStrategy(),
        new HttpWaitStrategy().ForPort(HttpPort),
        new HttpWaitStrategy().ForStatusCode(HttpStatusCode.OK),
        new HttpWaitStrategy().ForStatusCodeMatching(statusCode => HttpStatusCode.OK.Equals(statusCode)),
        new HttpWaitStrategy().ForResponseMessageMatching(response => Task.FromResult(response.IsSuccessStatusCode)),
        new HttpWaitStrategy().ForStatusCode(HttpStatusCode.MovedPermanently).ForStatusCodeMatching(statusCode => HttpStatusCode.OK.Equals(statusCode)),
      };

    [Theory]
    [MemberData(nameof(HttpWaitStrategies))]
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

      await Task.Delay(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken)
        .ConfigureAwait(true);

      var (_, stderr) = await _container.GetLogsAsync(ct: TestContext.Current.CancellationToken)
        .ConfigureAwait(true);

      // Then
      Assert.True(succeeded);
      Assert.Contains("Authorization", stderr);
      Assert.Contains("QWxhZGRpbjpvcGVuIHNlc2FtZQ==", stderr);
      Assert.Contains(httpHeaders.First().Key, stderr);
      Assert.Contains(httpHeaders.First().Value, stderr);
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

      await Task.Delay(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken)
        .ConfigureAwait(true);

      var (_, stderr) = await _container.GetLogsAsync(ct: TestContext.Current.CancellationToken)
        .ConfigureAwait(true);

      // Then
      Assert.True(succeeded);
      Assert.Contains("Cookie", stderr);
      Assert.Contains("Key1=Value1", stderr);
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

    [UsedImplicitly]
    public sealed class HttpFixture : IAsyncLifetime
    {
      public IContainer Container { get; } = new ContainerBuilder()
        .WithImage(CommonImages.Socat)
        .WithCommand("-v")
        .WithCommand($"TCP-LISTEN:{HttpPort},crlf,reuseaddr,fork")
        .WithCommand("SYSTEM:'echo -e \"HTTP/1.1 200 OK\\nContent-Length: 0\\n\\n\"'")
        .WithPortBinding(HttpPort, true)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request => request))
        .Build();

      public ValueTask InitializeAsync()
      {
        return new ValueTask(Container.StartAsync());
      }

      public ValueTask DisposeAsync()
      {
        return Container.DisposeAsync();
      }
    }
  }
}
