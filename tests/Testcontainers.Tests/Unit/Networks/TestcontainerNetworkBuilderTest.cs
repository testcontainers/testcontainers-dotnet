namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Networks;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging.Abstractions;
  using Xunit;

  public sealed class TestcontainerNetworkBuilderTest : IClassFixture<TestcontainerNetworkBuilderTest.DockerNetwork>
  {
    private static readonly string NetworkName = Guid.NewGuid().ToString("D");

    private static readonly KeyValuePair<string, string> Option = new KeyValuePair<string, string>("mtu", "1350");

    private static readonly KeyValuePair<string, string> Label = new KeyValuePair<string, string>(TestcontainersClient.TestcontainersLabel + ".network.test", Guid.NewGuid().ToString("D"));

    private static readonly KeyValuePair<string, string> ParameterModifier = new KeyValuePair<string, string>(TestcontainersClient.TestcontainersLabel + ".parameter.modifier", Guid.NewGuid().ToString("D"));

    private readonly INetwork _network;

    public TestcontainerNetworkBuilderTest(DockerNetwork network)
    {
      _network = network;
    }

    [Fact]
    public void GetNameThrowsInvalidOperationException()
    {
      _ = Assert.Throws<InvalidOperationException>(() => new NetworkBuilder()
        .WithName(NetworkName)
        .Build()
        .Name);
    }

    [Fact]
    public void GetNameReturnsNetworkName()
    {
      Assert.Equal(NetworkName, _network.Name);
    }

    [Fact]
    public async Task CreateNetworkAssignsOptions()
    {
      // Given
      var client = new TestcontainersClient(ResourceReaper.DefaultSessionId, TestcontainersSettings.OS.DockerEndpointAuthConfig, NullLogger.Instance);

      // When
      var networkResponse = await client.Network.ByIdAsync(_network.Name)
        .ConfigureAwait(true);

      // Then
      Assert.Equal(Option.Value, Assert.Contains(Option.Key, networkResponse.Options));
    }

    [Fact]
    public async Task CreateNetworkAssignsLabels()
    {
      // Given
      var client = new TestcontainersClient(ResourceReaper.DefaultSessionId, TestcontainersSettings.OS.DockerEndpointAuthConfig, NullLogger.Instance);

      // When
      var networkResponse = await client.Network.ByIdAsync(_network.Name)
        .ConfigureAwait(true);

      // Then
      Assert.Equal(Label.Value, Assert.Contains(Label.Key, networkResponse.Labels));
      Assert.Equal(ParameterModifier.Value, Assert.Contains(ParameterModifier.Key, networkResponse.Labels));
    }

    [UsedImplicitly]
    public sealed class DockerNetwork : INetwork, IAsyncLifetime
    {
      private readonly INetwork _network = new NetworkBuilder()
        .WithName(NetworkName)
        .WithOption(Option.Key, Option.Value)
        .WithLabel(Label.Key, Label.Value)
        .WithCreateParameterModifier(parameterModifier => parameterModifier.Labels.Add(ParameterModifier.Key, ParameterModifier.Value))
        .Build();

      public string Name => _network.Name;

      public Task InitializeAsync()
      {
        return CreateAsync();
      }

      public Task DisposeAsync()
      {
        IAsyncDisposable asyncDisposable = this;
        return asyncDisposable.DisposeAsync().AsTask();
      }

      public Task CreateAsync(CancellationToken ct = default)
      {
        return _network.CreateAsync(ct);
      }

      public Task DeleteAsync(CancellationToken ct = default)
      {
        return _network.DeleteAsync(ct);
      }

      ValueTask IAsyncDisposable.DisposeAsync()
      {
        return _network.DisposeAsync();
      }
    }
  }
}
