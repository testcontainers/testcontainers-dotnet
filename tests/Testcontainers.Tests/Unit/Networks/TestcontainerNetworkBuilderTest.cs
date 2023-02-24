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

    private static readonly KeyValuePair<string, string> Option = new KeyValuePair<string, string>("com.docker.network.driver.mtu", "1350");

    private static readonly KeyValuePair<string, string> Label = new KeyValuePair<string, string>(TestcontainersClient.TestcontainersLabel + ".network.test", Guid.NewGuid().ToString("D"));

    private static readonly KeyValuePair<string, string> ParameterModifier = new KeyValuePair<string, string>(TestcontainersClient.TestcontainersLabel + ".parameter.modifier", Guid.NewGuid().ToString("D"));

    private readonly INetwork network;

    public TestcontainerNetworkBuilderTest(DockerNetwork network)
    {
      this.network = network;
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
      Assert.Equal(NetworkName, this.network.Name);
    }

    [Fact]
    public async Task CreateNetworkAssignsOptions()
    {
      IDockerNetworkOperations networkOperations = new DockerNetworkOperations(ResourceReaper.DefaultSessionId, TestcontainersSettings.OS.DockerEndpointAuthConfig, NullLogger.Instance);

      // When
      var networkResponse = await networkOperations.ByNameAsync(this.network.Name)
        .ConfigureAwait(false);

      // Then
      Assert.Equal(Option.Value, Assert.Contains(Option.Key, networkResponse.Options));
    }

    [Fact]
    public async Task CreateNetworkAssignsLabels()
    {
      // Given
      IDockerNetworkOperations networkOperations = new DockerNetworkOperations(ResourceReaper.DefaultSessionId, TestcontainersSettings.OS.DockerEndpointAuthConfig, NullLogger.Instance);

      // When
      var networkResponse = await networkOperations.ByNameAsync(this.network.Name)
        .ConfigureAwait(false);

      // Then
      Assert.Equal(Label.Value, Assert.Contains(Label.Key, networkResponse.Labels));
      Assert.Equal(ParameterModifier.Value, Assert.Contains(ParameterModifier.Key, networkResponse.Labels));
    }

    [UsedImplicitly]
    public sealed class DockerNetwork : INetwork, IAsyncLifetime
    {
      private readonly INetwork network = new NetworkBuilder()
        .WithName(NetworkName)
        .WithOption(Option.Key, Option.Value)
        .WithLabel(Label.Key, Label.Value)
        .WithCreateParameterModifier(parameterModifier => parameterModifier.Labels.Add(ParameterModifier.Key, ParameterModifier.Value))
        .Build();

      public string Name
      {
        get
        {
          return this.network.Name;
        }
      }

      public Task InitializeAsync()
      {
        return this.CreateAsync();
      }

      public Task DisposeAsync()
      {
        return this.DeleteAsync();
      }

      public Task CreateAsync(CancellationToken ct = default)
      {
        return this.network.CreateAsync(ct);
      }

      public Task DeleteAsync(CancellationToken ct = default)
      {
        return this.network.DeleteAsync(ct);
      }
    }
  }
}
