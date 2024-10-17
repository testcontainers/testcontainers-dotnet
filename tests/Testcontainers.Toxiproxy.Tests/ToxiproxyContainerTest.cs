using Toxiproxy.Net;
using Toxiproxy.Net.Toxics;

namespace Testcontainers.Toxiproxy
{
    public sealed class ToxiproxyContainerTest : IAsyncLifetime
    {
        private readonly ToxiproxyContainer _toxiproxyContainer = new ToxiproxyBuilder().Build();

        public Task InitializeAsync()
        {
            // Start the Toxiproxy container
            return _toxiproxyContainer.StartAsync();
        }

        public Task DisposeAsync()
        {
            // Dispose the container when the test finishes
            return _toxiproxyContainer.DisposeAsync().AsTask();
        }

        [Fact]
        public void CanCreateAndFindProxy()
        {
            // Arrange
            var connection = new Connection(_toxiproxyContainer.GetHost(), _toxiproxyContainer.GetControlPort());
            var client = connection.Client();

            // Create a proxy for traffic from 127.0.0.1:44399 to google.com:443
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
            var connection = new Connection(_toxiproxyContainer.GetHost(), _toxiproxyContainer.GetControlPort());
            var client = connection.Client();

            // Create two proxies
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
          var connection = new Connection(_toxiproxyContainer.GetHost(), _toxiproxyContainer.GetControlPort());
          var client = connection.Client();

          // Create and add a proxy
          var proxy = new Proxy
          {
            Name = "proxyToDelete",
            Enabled = true,
            Listen = "127.0.0.1:44402",
            Upstream = "delete.com:80"
          };

          var addedProxy = client.Add(proxy);

          // Act - Delete the proxy
          addedProxy.Delete();

          // Assert - Verify that the proxy no longer exists
          Assert.Throws<ToxiProxiException>(() => client.FindProxy("proxyToDelete"));
        }

        [Fact]
        public void CanAddSlowCloseToxic()
        {
          // Arrange
          var connection = new Connection(_toxiproxyContainer.GetHost(), _toxiproxyContainer.GetControlPort());
          var client = connection.Client();

          // Create and add a proxy
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
            Toxicity = 0.8  // Setting the toxicity level
          };
          slowCloseToxic.Attributes.Delay = 50;  // Delay in milliseconds

          // Add the toxic to the proxy and update
          addedProxy.Add(slowCloseToxic);
          addedProxy.Update();

          // Act - Retrieve all toxics from the proxy
          var toxics = addedProxy.GetAllToxics().ToList();

          // Assert - Check if the toxic was correctly added
          Assert.Single(toxics);
          var retrievedToxic = toxics.First() as SlowCloseToxic;
          Assert.NotNull(retrievedToxic);
          Assert.Equal("slowCloseToxic", retrievedToxic.Name);
          Assert.Equal(50, retrievedToxic.Attributes.Delay);
          Assert.Equal(ToxicDirection.DownStream, retrievedToxic.Stream);
        }

    }
}
