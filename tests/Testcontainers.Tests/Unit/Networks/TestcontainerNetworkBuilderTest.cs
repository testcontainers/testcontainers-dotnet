namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Networks;
  using JetBrains.Annotations;
  using Xunit;

  public sealed class TestcontainerNetworkBuilderTest : IClassFixture<TestcontainerNetworkBuilderTest.DockerNetwork>
  {
    private static readonly string NetworkName = Guid.NewGuid().ToString("D");

    private static readonly KeyValuePair<string, string> Label = new KeyValuePair<string, string>(TestcontainersClient.TestcontainersLabel + ".network.test", Guid.NewGuid().ToString("D"));

    private static readonly KeyValuePair<string, string> Option = new KeyValuePair<string, string>("com.docker.network.driver.mtu", "1350");

    private readonly IDockerNetwork network;

    public TestcontainerNetworkBuilderTest(DockerNetwork network)
    {
      this.network = network;
    }

    [Fact]
    public void GetIdOrNameThrowsInvalidOperationException()
    {
      var noSuchNetwork = new TestcontainersNetworkBuilder()
        .WithName(NetworkName)
        .Build();

      Assert.Throws<InvalidOperationException>(() => noSuchNetwork.Id);
      Assert.Throws<InvalidOperationException>(() => noSuchNetwork.Name);
    }

    [Fact]
    public void GetIdReturnsNetworkId()
    {
      Assert.NotEmpty(this.network.Id);
    }

    [Fact]
    public void GetNameReturnsNetworkName()
    {
      Assert.Equal(NetworkName, this.network.Name);
    }

    [Fact]
    public async Task CreateNetworkAssignsLabels()
    {
      // Given
      using var dockerClientConfiguration = TestcontainersSettings.OS.DockerEndpointAuthConfig.GetDockerClientConfiguration();
      using var dockerClient = dockerClientConfiguration.CreateClient();

      // When
      var networkResponse = await dockerClient.Networks.InspectNetworkAsync(this.network.Id)
        .ConfigureAwait(false);

      // Then
      Assert.Equal(Label.Value, Assert.Contains(Label.Key, networkResponse.Labels));
    }

    [Fact]
    public async Task CreateNetworkAssignsOptions()
    {
      // Given
      using var dockerClientConfiguration = TestcontainersSettings.OS.DockerEndpointAuthConfig.GetDockerClientConfiguration();
      using var dockerClient = dockerClientConfiguration.CreateClient();

      // When
      var networkResponse = await dockerClient.Networks.InspectNetworkAsync(this.network.Id)
        .ConfigureAwait(false);

      // Then
      Assert.Equal(Option.Value, Assert.Contains(Option.Key, networkResponse.Options));
    }

    [UsedImplicitly]
    public sealed class DockerNetwork : IDockerNetwork, IAsyncLifetime
    {
      private readonly IDockerNetwork network = new TestcontainersNetworkBuilder()
        .WithName(NetworkName)
        .WithLabel(Label.Key, Label.Value)
        .WithOption(Option.Key, Option.Value)
        .Build();

      public string Id
      {
        get
        {
          return this.network.Id;
        }
      }

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
