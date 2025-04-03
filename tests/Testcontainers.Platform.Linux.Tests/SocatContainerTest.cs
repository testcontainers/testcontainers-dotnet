namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Net.Http;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Commons;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Networks;
  using Xunit;

  public sealed class SocatContainerTest : IAsyncLifetime
  {
    private const string HelloWorldAlias = "hello-world-container";

    private readonly INetwork _network;

    private readonly IContainer _helloWorldContainer;

    private readonly IContainer _socatContainer;

    public SocatContainerTest()
    {
      _network = new NetworkBuilder()
        .Build();

      _helloWorldContainer = new ContainerBuilder()
        .WithImage(CommonImages.HelloWorld)
        .WithNetwork(_network)
        .WithNetworkAliases(HelloWorldAlias)
        .Build();

      _socatContainer = new SocatBuilder()
        .WithNetwork(_network)
        .WithTarget(8080, HelloWorldAlias)
        .WithTarget(8081, HelloWorldAlias, 8080)
        .Build();
    }

    public async Task InitializeAsync()
    {
      await _helloWorldContainer.StartAsync()
        .ConfigureAwait(false);

      await _socatContainer.StartAsync()
        .ConfigureAwait(false);
    }

    public async Task DisposeAsync()
    {
      await _socatContainer.DisposeAsync()
        .ConfigureAwait(false);

      await _helloWorldContainer.DisposeAsync()
        .ConfigureAwait(false);

      await _network.DisposeAsync()
        .ConfigureAwait(false);
    }

    [Theory]
    [InlineData(8080)]
    [InlineData(8081)]
    public async Task RequestTargetContainer(int containerPort)
    {
      // Given
      using var httpClient = new HttpClient();
      httpClient.BaseAddress = new UriBuilder(Uri.UriSchemeHttp, _socatContainer.Hostname, _socatContainer.GetMappedPublicPort(containerPort)).Uri;

      // When
      using var httpResponse = await httpClient.GetAsync("/ping")
        .ConfigureAwait(true);

      var response = await httpResponse.Content.ReadAsStringAsync()
        .ConfigureAwait(true);

      // Then
      Assert.Equal("PONG", response);
    }
  }
}
