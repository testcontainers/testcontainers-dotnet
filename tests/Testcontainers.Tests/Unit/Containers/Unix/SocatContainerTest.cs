namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Net.Http;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Networks;
  using Xunit;

  public sealed class SocatContainerTest : IAsyncLifetime
  {
    private readonly INetwork _network;

    private readonly IContainer _helloworld;

    private SocatContainer _socat;

    public SocatContainerTest()
    {
      _network = new NetworkBuilder()
        .Build();

      _helloworld = new ContainerBuilder()
        .WithImage("testcontainers/helloworld:1.2.0")
        .WithPortBinding(8080, true)
        .WithNetwork(_network)
        .WithNetworkAliases("helloworld")
        .Build();
    }

    [Fact]
    public async Task RequestTargetContainer()
    {
      _socat = new SocatBuilder()
        .WithTarget(8080, "helloworld")
        .WithNetwork(_network)
        .Build();
      await _socat.StartAsync();

      using var httpClient = new HttpClient();
      var endpoint = new UriBuilder(Uri.UriSchemeHttp, _socat.Hostname, _socat.GetMappedPublicPort(8080)).ToString();
      httpClient.BaseAddress = new Uri(endpoint);

      using var httpResponse = await httpClient.GetAsync("/ping")
        .ConfigureAwait(true);

      var response = await httpResponse.Content.ReadAsStringAsync()
        .ConfigureAwait(true);

      Assert.Equal("PONG", response);

      await _socat.DisposeAsync();
    }

    [Fact]
    public async Task RequestTargetContainerWithDifferentPort()
    {
      _socat = new SocatBuilder()
        .WithTarget(8081, "helloworld", 8080)
        .WithNetwork(_network)
        .Build();
      await _socat.StartAsync();

      using var httpClient = new HttpClient();
      var endpoint = new UriBuilder(Uri.UriSchemeHttp, _socat.Hostname, _socat.GetMappedPublicPort(8081)).ToString();
      httpClient.BaseAddress = new Uri(endpoint);

      using var httpResponse = await httpClient.GetAsync("/ping")
        .ConfigureAwait(true);

      var response = await httpResponse.Content.ReadAsStringAsync()
        .ConfigureAwait(true);

      Assert.Equal("PONG", response);

      await _socat.DisposeAsync();
    }

    public async Task InitializeAsync()
    {
      await _helloworld.StartAsync()
        .ConfigureAwait(false);
    }

    public async Task DisposeAsync()
    {
      await _helloworld.DisposeAsync()
        .ConfigureAwait(false);

      await _network.DisposeAsync()
        .ConfigureAwait(false);
    }
  }
}
