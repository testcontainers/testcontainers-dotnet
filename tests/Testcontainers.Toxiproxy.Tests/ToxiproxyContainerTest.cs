namespace Testcontainers.Toxiproxy;

/// <summary>
/// Integration tests for the Toxiproxy container module.
/// </summary>
public sealed class ToxiproxyContainerTest : IAsyncLifetime
{
    private readonly ToxiproxyContainer _toxiproxyContainer = new ToxiproxyBuilder().Build();

    /// <inheritdoc />
    public Task InitializeAsync()
    {
        return _toxiproxyContainer.StartAsync();
    }

    /// <inheritdoc />
    public Task DisposeAsync()
    {
        return _toxiproxyContainer.DisposeAsync().AsTask();
    }

    [Fact]
    public void CanCreateAndFindProxy()
    {
        // Arrange
        var client = _toxiproxyContainer.Client;

        var proxy = new Proxy
        {
            Name = "localToGoogle",
            Enabled = true,
            Listen = "127.0.0.1:44399",
            Upstream = "google.com:443"
        };

        // Act
        client.Add(proxy);

        // Assert
        var retrievedProxy = client.FindProxy(proxy.Name);
        Assert.NotNull(retrievedProxy);
        Assert.Equal("localToGoogle", retrievedProxy.Name);
        Assert.Equal("127.0.0.1:44399", retrievedProxy.Listen);
        Assert.Equal("google.com:443", retrievedProxy.Upstream);
    }

    [Fact]
    public void CanFindAllProxies()
    {
        // Arrange
        var client = _toxiproxyContainer.Client;

        var proxyOne = new Proxy
        {
            Name = "proxyOne",
            Enabled = true,
            Listen = "127.0.0.1:44400",
            Upstream = "example.com:80"
        };

        var proxyTwo = new Proxy
        {
            Name = "proxyTwo",
            Enabled = true,
            Listen = "127.0.0.1:44401",
            Upstream = "test.com:80"
        };

        client.Add(proxyOne);
        client.Add(proxyTwo);

        // Act
        var allProxies = client.All();

        // Assert
        Assert.Equal(2, allProxies.Keys.Count);
        Assert.True(allProxies.ContainsKey("proxyOne"));
        Assert.True(allProxies.ContainsKey("proxyTwo"));
    }

    [Fact]
    public void CanDeleteProxy()
    {
        // Arrange
        var client = _toxiproxyContainer.Client;

        var proxy = new Proxy
        {
            Name = "proxyToDelete",
            Enabled = true,
            Listen = "127.0.0.1:44402",
            Upstream = "delete.com:80"
        };

        var addedProxy = client.Add(proxy);

        // Act
        addedProxy.Delete();

        // Assert
        Assert.Throws<ToxiProxiException>(() => client.FindProxy("proxyToDelete"));
    }

    [Fact]
    public void CanAddSlowCloseToxic()
    {
        // Arrange
        var client = _toxiproxyContainer.Client;

        var proxy = new Proxy
        {
            Name = "proxyWithToxic",
            Enabled = true,
            Listen = "127.0.0.1:44403",
            Upstream = "toxic.com:80"
        };

        var addedProxy = client.Add(proxy);

        // Add a SlowCloseToxic to the proxy
        var slowCloseToxic = new SlowCloseToxic
        {
            Name = "slowCloseToxic",
            Stream = ToxicDirection.DownStream,
            Toxicity = 0.8
        };
        slowCloseToxic.Attributes.Delay = 50;

        addedProxy.Add(slowCloseToxic);
        addedProxy.Update();

        // Act
        var toxics = addedProxy.GetAllToxics().ToList();

        // Assert
        Assert.Single(toxics);
        var retrievedToxic = toxics.First() as SlowCloseToxic;
        Assert.NotNull(retrievedToxic);
        Assert.Equal("slowCloseToxic", retrievedToxic.Name);
        Assert.Equal(50, retrievedToxic.Attributes.Delay);
        Assert.Equal(ToxicDirection.DownStream, retrievedToxic.Stream);
    }

    [Fact]
    public void CreatingDuplicateProxyThrows()
    {
      // Arrange
      var client = _toxiproxyContainer.Client;
      var proxy = new Proxy
      {
        Name = "duplicate",
        Enabled = true,
        Listen = "127.0.0.1:44500",
        Upstream = "service:80"
      };
      client.Add(proxy);

      // Act & Assert
      Assert.Throws<ToxiProxiException>(() => client.Add(proxy));
    }

    [Fact]
    public void InvalidListenAddressThrows()
    {
      var client = _toxiproxyContainer.Client;

      var proxy = new Proxy
      {
        Name = "invalidListen",
        Enabled = true,
        Listen = "notaport",
        Upstream = "localhost:1234"
      };

      Assert.Throws<ToxiProxiException>(() => client.Add(proxy));
    }

    [Fact]
    public void CanDisableProxy()
    {
      var client = _toxiproxyContainer.Client;

      var proxy = new Proxy
      {
        Name = "disabledProxy",
        Enabled = true,
        Listen = "127.0.0.1:44501",
        Upstream = "service.com:80"
      };

      var added = client.Add(proxy);
      added.Enabled = false;
      added.Update();

      var updated = client.FindProxy("disabledProxy");
      Assert.False(updated.Enabled);
    }

    [Fact]
    public void CanRemoveAllToxics()
    {
      var client = _toxiproxyContainer.Client;

      var proxy = new Proxy
      {
        Name = "proxyWithToxics",
        Enabled = true,
        Listen = "127.0.0.1:44503",
        Upstream = "api:80"
      };

      var added = client.Add(proxy);

      var toxic = new SlowCloseToxic
      {
        Name = "slow",
        Stream = ToxicDirection.DownStream,
        Toxicity = 1.0,
        Attributes = { Delay = 100 }
      };

      added.Add(toxic);
      added.RemoveToxic("slow");

      var toxics = added.GetAllToxics();
      Assert.Empty(toxics);
    }

    [Fact]
    public async Task LatencyToxicConfigurationIsApplied()
    {
      var container = new ToxiproxyBuilder()
        .WithProxy("latency", "0.0.0.0:12345", "localhost:12346")
        .WithPortBinding(12345, true)
        .WithWaitStrategy(Wait.ForUnixContainer()
          .UntilHttpRequestIsSucceeded(req => req
            .ForPort(8474)
            .ForPath("/proxies")))
        .Build();

      await container.StartAsync();
      await WaitForProxyToBeReady(container.Client, "latency", TimeSpan.FromSeconds(10));

      var proxy = container.Client.FindProxy("latency");

      proxy.Add(new LatencyToxic
      {
        Name = "latency-toxic",
        Stream = ToxicDirection.DownStream,
        Attributes = { Latency = 500 }
      });

      await Task.Delay(250);

      var updated = container.Client.FindProxy("latency");
      var toxics = updated.GetAllToxics();
      var toxic = toxics.FirstOrDefault(t => t.Name == "latency-toxic") as LatencyToxic;

      Assert.NotNull(toxic);
      Assert.Equal(500, toxic.Attributes.Latency);
    }

    [Fact]
    public async Task LatencyToxic_ShouldIntroduceExpectedDelay()
    {
        var serverPort = GetFreePort();
        var listener = new TcpListener(IPAddress.Loopback, serverPort);
        listener.Start();

        _ = Task.Run(async () =>
        {
            using var serverClient = await listener.AcceptTcpClientAsync();
            using var stream = serverClient.GetStream();
            var buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer);
            await stream.WriteAsync(buffer, 0, bytesRead);
        });

        var proxyPort = GetFreePort();
        var proxyName = "latency-proxy";
        var container = new ToxiproxyBuilder()
            .WithProxy(proxyName, $"0.0.0.0:{proxyPort}", $"host.docker.internal:{serverPort}")
            .WithPortBinding(proxyPort, false)
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilHttpRequestIsSucceeded(req => req
                    .ForPort(8474)
                    .ForPath("/proxies")
                    .ForStatusCode(HttpStatusCode.OK)))
            .Build();

        await container.StartAsync();

        var proxy = container.Client.FindProxy(proxyName);
        proxy.Add(new LatencyToxic
        {
            Name = "latency",
            Stream = ToxicDirection.DownStream,
            Toxicity = 1.0,
            Attributes = { Latency = 500 }
        });

        using var client = new TcpClient();
        await client.ConnectAsync("127.0.0.1", proxyPort);
        using var stream = client.GetStream();

        var message = Encoding.UTF8.GetBytes("test");
        var buffer = new byte[message.Length];

        var sw = Stopwatch.StartNew();
        await stream.WriteAsync(message);
        await stream.ReadAsync(buffer);
        sw.Stop();

        var delay = sw.ElapsedMilliseconds;

        await container.DisposeAsync();
        listener.Stop();

        const int expectedMin = 450;
        const int expectedMax = 2000;
        Assert.True(delay >= expectedMin && delay <= expectedMax,
            $"Expected delay between {expectedMin}ms and {expectedMax}ms, but got {delay}ms");
    }


    private static async Task WaitForProxyToBeActive(Client client, string proxyName, string host, int port, TimeSpan timeout)
    {
      var start = DateTime.UtcNow;
      Exception? lastException = null;

      while (DateTime.UtcNow - start < timeout)
      {
        try
        {
          var proxy = client.FindProxy(proxyName);
          if (proxy.Enabled)
          {
            using var tcp = new TcpClient();
            await tcp.ConnectAsync(host, port);
            return;
          }
        }
        catch (Exception ex)
        {
          lastException = ex;
          await Task.Delay(100);
        }
      }

      throw new TimeoutException($"Proxy '{proxyName}' on {host}:{port} did not become ready in time.", lastException);
    }

    [Fact]
    public async Task TimeoutToxic_ShouldDropConnectionAfterTimeout()
    {
        var serverPort = GetFreePort();
        var listener = new TcpListener(IPAddress.Loopback, serverPort);
        listener.Start();

        _ = Task.Run(async () =>
        {
            using var serverClient = await listener.AcceptTcpClientAsync();
            using var stream = serverClient.GetStream();
            var buffer = new byte[1024];
            await stream.ReadAsync(buffer);
        });

        var proxyPort = GetFreePort();
        var proxyName = "timeout-proxy";

        var container = new ToxiproxyBuilder()
            .WithProxy(proxyName, $"0.0.0.0:{proxyPort}", $"host.docker.internal:{serverPort}")
            .WithPortBinding(proxyPort, false)
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilHttpRequestIsSucceeded(req => req
                    .ForPort(8474)
                    .ForPath("/proxies")
                    .ForStatusCode(HttpStatusCode.OK)))
            .Build();

        await container.StartAsync();
        var proxy = container.Client.FindProxy(proxyName);

        proxy.Add(new TimeoutToxic
        {
            Name = "timeout-toxic",
            Stream = ToxicDirection.UpStream,
            Toxicity = 1.0,
            Attributes = { Timeout = 1000 }
        });

        using var client = new TcpClient();
        await client.ConnectAsync("127.0.0.1", proxyPort);

        using var stream = client.GetStream();
        var payload = Encoding.UTF8.GetBytes("test");

        var sw = Stopwatch.StartNew();
        Exception? ex = await Record.ExceptionAsync(async () =>
        {
            await stream.WriteAsync(payload);
            await stream.ReadAsync(new byte[5]);
        });
        sw.Stop();

        await container.DisposeAsync();
        listener.Stop();
        Assert.True(sw.ElapsedMilliseconds >= 1000, $"Expected timeout after >= 1000ms but took {sw.ElapsedMilliseconds}ms.");
    }


    private static async Task WaitForProxyToBeReady(Client client, string proxyName, TimeSpan timeout)
    {
      var start = DateTime.UtcNow;
      Exception? lastError = null;

      while (DateTime.UtcNow - start < timeout)
      {
        try
        {
          var proxy = client.FindProxy(proxyName);
          if (!string.IsNullOrEmpty(proxy.Listen))
          {
            return;
          }
        }
        catch (Exception ex)
        {
          lastError = ex;
        }

        await Task.Delay(100);
      }

      throw new TimeoutException($"Proxy '{proxyName}' did not become ready in time.", lastError);
    }

    private static int GetFreePort()
    {
      var listener = new TcpListener(IPAddress.Loopback, 0);
      listener.Start();
      var port = ((IPEndPoint)listener.LocalEndpoint).Port;
      listener.Stop();
      return port;
    }
}
