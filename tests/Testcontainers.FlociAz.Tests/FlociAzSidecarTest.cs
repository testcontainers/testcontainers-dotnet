namespace Testcontainers.FlociAz;

[Collection(nameof(FlociAzSidecarCollection))]
public sealed class FlociAzSidecarTest(FlociAzSidecarTest.FlociAzSidecarFixture fixture)
    : IClassFixture<FlociAzSidecarTest.FlociAzSidecarFixture>
{
    private const string AzureService = "Service";

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AzureService, "servicebus")]
    public async Task ServiceBusSendsAndReceivesMessage()
    {
        // Given
        var queueName = "queue-" + Guid.NewGuid().ToString("N");
        using var managementClient = fixture.CreateHttpClient("servicebus");
        using var createResponse = await managementClient.PutAsync(queueName, new StringContent("<QueueDescription xmlns=\"http://schemas.microsoft.com/netservices/2010/10/servicebus/connect\" />", Encoding.UTF8, "application/atom+xml"), TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        await AssertSuccessAsync(createResponse).ConfigureAwait(true);

        await using var client = new ServiceBusClient(CreateAmqpConnectionString(fixture.Container.Hostname, 5673));
        await using var sender = client.CreateSender(queueName);
        await using var receiver = client.CreateReceiver(queueName);
        var messageBody = Guid.NewGuid().ToString("D");

        // When
        await sender.SendMessageAsync(new ServiceBusMessage(messageBody), TestContext.Current.CancellationToken).ConfigureAwait(true);
        var message = await receiver.ReceiveMessageAsync(TimeSpan.FromSeconds(20), TestContext.Current.CancellationToken).ConfigureAwait(true);

        // Then
        Assert.NotNull(message);
        Assert.Equal(messageBody, message.Body.ToString());
        await receiver.CompleteMessageAsync(message, TestContext.Current.CancellationToken).ConfigureAwait(true);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AzureService, "redis")]
    public async Task RedisSidecarStoresAndReturnsValue()
    {
        // Given
        var cacheName = "redis" + Guid.NewGuid().ToString("N");
        var path = fixture.GetArmPath($"Microsoft.Cache/redis/{cacheName}?api-version=2024-11-01");
        using var client = fixture.CreateHttpClient();
        var payload = new { location = "eastus", properties = new { sku = new { name = "Basic", family = "C", capacity = 0 }, enableNonSslPort = true } };

        // When
        using var createResponse = await client.PutAsJsonAsync(path, payload, TestContext.Current.CancellationToken).ConfigureAwait(true);
        using var cache = await ReadJsonAsync(createResponse).ConfigureAwait(true);
        var properties = cache.RootElement.GetProperty("properties");
        var sidecarHostname = properties.GetProperty("hostName").GetString();
        var privatePort = properties.GetProperty("port").GetUInt16();
        var publicPort = await fixture.Container.GetSidecarMappedPublicPortAsync(sidecarHostname, privatePort, TestContext.Current.CancellationToken).ConfigureAwait(true);
        var password = properties.GetProperty("accessKeys").GetProperty("primaryKey").GetString();

        await using var connection = await ConnectionMultiplexer.ConnectAsync($"{fixture.Container.Hostname}:{publicPort},password={password},abortConnect=false").ConfigureAwait(true);
        var database = connection.GetDatabase();
        var key = Guid.NewGuid().ToString("D");
        var value = Guid.NewGuid().ToString("D");
        _ = await database.StringSetAsync(key, value).ConfigureAwait(true);

        // Then
        Assert.Equal(value, await database.StringGetAsync(key).ConfigureAwait(true));

        using var deleteResponse = await client.DeleteAsync(path, TestContext.Current.CancellationToken).ConfigureAwait(true);
        await AssertSuccessAsync(deleteResponse).ConfigureAwait(true);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AzureService, "postgresql")]
    public async Task PostgreSqlSidecarExecutesQuery()
    {
        // Given
        var serverName = "pg" + Guid.NewGuid().ToString("N");
        var path = fixture.GetArmPath($"Microsoft.DBforPostgreSQL/flexibleServers/{serverName}?api-version=2025-08-01");
        using var client = fixture.CreateHttpClient();
        var payload = new
        {
            location = "eastus",
            sku = new { name = "Standard_B1ms", tier = "Burstable" },
            properties = new { administratorLogin = "psqladmin", administratorLoginPassword = "FlociAz_Strong123!", version = "16", storage = new { storageSizeGB = 32 } },
        };

        // When
        using var createResponse = await client.PutAsJsonAsync(path, payload, TestContext.Current.CancellationToken).ConfigureAwait(true);
        await AssertSuccessAsync(createResponse).ConfigureAwait(true);
        using var postgresClient = fixture.CreateHttpClient("postgres");
        using var connectResponse = await postgresClient.GetAsync($"flexibleServers/{serverName}/connect", TestContext.Current.CancellationToken).ConfigureAwait(true);
        using var connectionInfo = await ReadJsonAsync(connectResponse).ConfigureAwait(true);
        var sidecarHostname = connectionInfo.RootElement.GetProperty("host").GetString();
        var privatePort = connectionInfo.RootElement.GetProperty("port").GetUInt16();
        var publicPort = await fixture.Container.GetSidecarMappedPublicPortAsync(sidecarHostname, privatePort, TestContext.Current.CancellationToken).ConfigureAwait(true);

        var connectionString = $"Host={fixture.Container.Hostname};Port={publicPort};Database=postgres;Username=psqladmin;Password=FlociAz_Strong123!;SSL Mode=Disable";
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(TestContext.Current.CancellationToken).ConfigureAwait(true);
        await using var command = new NpgsqlCommand("SELECT 42", connection);

        // Then
        Assert.Equal(42, await command.ExecuteScalarAsync(TestContext.Current.CancellationToken).ConfigureAwait(true));

        using var deleteResponse = await client.DeleteAsync(path, TestContext.Current.CancellationToken).ConfigureAwait(true);
        await AssertSuccessAsync(deleteResponse).ConfigureAwait(true);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AzureService, "acr")]
    public async Task ContainerRegistrySidecarExposesRegistryApi()
    {
        // Given
        var registryName = "acr" + Guid.NewGuid().ToString("N");
        var path = fixture.GetArmPath($"Microsoft.ContainerRegistry/registries/{registryName}?api-version=2023-07-01");
        using var client = fixture.CreateHttpClient();

        // When
        using var createResponse = await client.PutAsJsonAsync(path, new { location = "eastus", sku = new { name = "Basic" }, properties = new { adminUserEnabled = true } }, TestContext.Current.CancellationToken).ConfigureAwait(true);
        using var registry = await ReadJsonAsync(createResponse).ConfigureAwait(true);
        var loginServer = registry.RootElement.GetProperty("properties").GetProperty("loginServer").GetString();
        var registryUri = new Uri(Uri.UriSchemeHttp + Uri.SchemeDelimiter + loginServer);
        var sidecarHostname = registryUri.Host;
        var privatePort = checked((ushort)registryUri.Port);
        var publicPort = await fixture.Container.GetSidecarMappedPublicPortAsync(sidecarHostname, privatePort, TestContext.Current.CancellationToken).ConfigureAwait(true);

        using var registryClient = new HttpClient { BaseAddress = new UriBuilder(Uri.UriSchemeHttp, fixture.Container.Hostname, publicPort).Uri };
        using var apiResponse = await registryClient.GetAsync("v2/", TestContext.Current.CancellationToken).ConfigureAwait(true);

        // Then
        await AssertSuccessAsync(apiResponse).ConfigureAwait(true);
        Assert.True(apiResponse.Headers.Contains("Docker-Distribution-Api-Version"));
    }

    private static string CreateAmqpConnectionString(string hostname, ushort port)
    {
        return $"Endpoint=sb://{hostname}:{port};SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=devkey;UseDevelopmentEmulator=true;";
    }

    private static async Task AssertSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken).ConfigureAwait(true);
        Assert.Fail($"{response.RequestMessage?.Method} {response.RequestMessage?.RequestUri} returned {(int)response.StatusCode}: {content}");
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        await AssertSuccessAsync(response).ConfigureAwait(true);
        return await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken).ConfigureAwait(true), cancellationToken: TestContext.Current.CancellationToken).ConfigureAwait(true);
    }

    public sealed class FlociAzSidecarFixture : IAsyncLifetime
    {
        private const string SubscriptionId = "00000000-0000-0000-0000-000000000003";

        private const string ResourceGroup = "sidecars";

        public FlociAzContainer Container { get; }
            = new FlociAzBuilder(TestSession.GetImageFromDockerfile())
                .WithDockerSocket()
                .WithEnvironment("FLOCI_AZ_SERVICES_SERVICE_BUS_MOCKED", "false")
                .WithEnvironment("FLOCI_AZ_SERVICES_SERVICE_BUS_START_ON_BOOT", "true")
                .WithEnvironment("FLOCI_AZ_SERVICES_POSTGRES_MOCKED", "false")
                .WithEnvironment("FLOCI_AZ_SERVICES_ACR_MOCKED", "false")
                .WithEnvironment("FLOCI_AZ_SERVICES_REDIS_MOCKED", "false")
                .Build();

        public async ValueTask InitializeAsync()
        {
            await Container.StartAsync().ConfigureAwait(false);
            using var client = CreateHttpClient();
            using var response = await client.PutAsJsonAsync($"subscriptions/{SubscriptionId}/resourceGroups/{ResourceGroup}?api-version=2021-04-01", new { location = "eastus" }, TestContext.Current.CancellationToken).ConfigureAwait(false);
            await AssertSuccessAsync(response).ConfigureAwait(false);
        }

        public ValueTask DisposeAsync()
        {
            return Container.DisposeAsync();
        }

        public HttpClient CreateHttpClient(string service = null)
        {
            return new HttpClient { BaseAddress = new Uri(service is null ? Container.GetEndpoint() : Container.GetServiceEndpoint(service)) };
        }

        public string GetArmPath(string resourcePath)
        {
            return $"subscriptions/{SubscriptionId}/resourceGroups/{ResourceGroup}/providers/{resourcePath}";
        }
    }
}

[CollectionDefinition(nameof(FlociAzSidecarCollection), DisableParallelization = true)]
public sealed class FlociAzSidecarCollection;
