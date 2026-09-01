namespace Testcontainers.FlociAz;

public sealed partial class FlociAzContainerTest(FlociAzContainerTest.FlociAzFixture fixture)
    : IClassFixture<FlociAzContainerTest.FlociAzFixture>
{
    private const string AzureService = "Service";

    public static TheoryData<string, string, string, string, bool> ArmResources
        => new()
        {
            {
                "Microsoft.Network/virtualNetworks/{name}?api-version=2024-05-01",
                """{"location":"eastus","properties":{"addressSpace":{"addressPrefixes":["10.0.0.0/16"]}}}""",
                "properties.provisioningState",
                "Succeeded",
                true
            },
            {
                "Microsoft.Compute/virtualMachines/{name}?api-version=2024-11-01",
                """{"location":"eastus","properties":{"hardwareProfile":{"vmSize":"Standard_B1s"},"storageProfile":{"imageReference":{"publisher":"Canonical","offer":"ubuntu","sku":"22_04-lts","version":"latest"}},"osProfile":{"computerName":"{name}","adminUsername":"azureuser"},"networkProfile":{"networkInterfaces":[]}}}""",
                "properties.hardwareProfile.vmSize",
                "Standard_B1s",
                true
            },
            {
                "Microsoft.Sql/servers/{name}?api-version=2021-11-01",
                """{"location":"eastus","properties":{"administratorLogin":"sa","administratorLoginPassword":"FlociAz_Strong123!"}}""",
                "properties.administratorLogin",
                "sa",
                true
            },
            {
                "Microsoft.ContainerService/managedClusters/{name}?api-version=2024-04-01",
                """{"location":"eastus","properties":{"kubernetesVersion":"1.29","dnsPrefix":"{name}","agentPoolProfiles":[{"name":"nodepool1","count":1,"vmSize":"Standard_DS2_v2","osType":"Linux","mode":"System"}]}}""",
                "properties.dnsPrefix",
                "{name}",
                true
            },
            {
                "Microsoft.ContainerRegistry/registries/{name}?api-version=2023-07-01",
                """{"location":"eastus","sku":{"name":"Basic"},"properties":{"adminUserEnabled":true}}""",
                "sku.name",
                "Basic",
                true
            },
            {
                "Microsoft.Cache/redis/{name}?api-version=2024-11-01",
                """{"location":"eastus","properties":{"sku":{"name":"Basic","family":"C","capacity":0},"enableNonSslPort":true}}""",
                "properties.sku.name",
                "Basic",
                true
            },
            {
                "Microsoft.ContainerInstance/containerGroups/{name}?api-version=2023-05-01",
                """{"location":"eastus","properties":{"osType":"Linux","containers":[{"name":"app","properties":{"image":"alpine:3.20","resources":{"requests":{"cpu":1,"memoryInGB":1}}}}]}}""",
                "properties.osType",
                "Linux",
                true
            },
            {
                "Microsoft.ApiManagement/service/{name}?api-version=2024-05-01",
                """{"location":"eastus","sku":{"name":"Developer","capacity":1},"properties":{"publisherEmail":"dev@example.com","publisherName":"FlociAz"}}""",
                "properties.publisherName",
                "FlociAz",
                true
            },
            {
                "Microsoft.ManagedIdentity/userAssignedIdentities/{name}?api-version=2024-11-30",
                """{"location":"eastus"}""",
                "location",
                "eastus",
                true
            },
            {
                "Microsoft.EventGrid/topics/{name}?api-version=2023-12-15-preview",
                """{"location":"eastus","properties":{}}""",
                "properties.provisioningState",
                "Succeeded",
                true
            },
            {
                "Microsoft.OperationalInsights/workspaces/{name}?api-version=2023-09-01",
                """{"location":"eastus","properties":{}}""",
                "properties.provisioningState",
                "Succeeded",
                false
            },
            {
                "Microsoft.Communication/communicationServices/{name}?api-version=2023-04-01",
                """{"location":"global","properties":{"dataLocation":"United States"}}""",
                "properties.dataLocation",
                "United States",
                true
            },
            {
                "Microsoft.DBforPostgreSQL/flexibleServers/{name}?api-version=2025-08-01",
                """{"location":"eastus","sku":{"name":"Standard_B1ms","tier":"Burstable"},"properties":{"administratorLogin":"psqladmin","administratorLoginPassword":"FlociAz_Strong123!","version":"16","storage":{"storageSizeGB":32}}}""",
                "properties.administratorLogin",
                "psqladmin",
                true
            },
        };

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ResolvingSidecarPortRequiresDockerSocket()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            _ = await fixture.Container.GetSidecarMappedPublicPortAsync("sidecar", 1234, TestContext.Current.CancellationToken).ConfigureAwait(true);
        });

        Assert.Contains(nameof(FlociAzBuilder.WithDockerSocket), exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AzureService, "blob")]
    public async Task DownloadBlobReturnsUploadedBlob()
    {
        // Given
        var content = Guid.NewGuid().ToString("D");

        var client = new BlobServiceClient(fixture.Container.GetConnectionString());

        var containerClient = client.GetBlobContainerClient(Guid.NewGuid().ToString("D"));

        var blobClient = containerClient.GetBlobClient(Guid.NewGuid().ToString("D"));

        // When
        _ = await containerClient.CreateAsync(cancellationToken: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        _ = await blobClient.UploadAsync(BinaryData.FromString(content), cancellationToken: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var downloadResult = await blobClient.DownloadContentAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(content, downloadResult.Value.Content.ToString());
        Assert.Equal(fixture.Container.GetConnectionString(), fixture.Container.GetConnectionString(ConnectionMode.Host));
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AzureService, "queue")]
    public async Task ReceiveMessageReturnsSentMessage()
    {
        // Given
        var message = Guid.NewGuid().ToString("D");

        var client = new QueueServiceClient(fixture.Container.GetConnectionString());

        var queueClient = client.GetQueueClient(Guid.NewGuid().ToString("D"));

        // When
        _ = await queueClient.CreateAsync(cancellationToken: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        _ = await queueClient.SendMessageAsync(message, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var receivedMessage = await queueClient.ReceiveMessageAsync(cancellationToken: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(message, receivedMessage.Value.MessageText);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AzureService, "table")]
    public async Task GetEntityReturnsAddedEntity()
    {
        // Given
        var partitionKey = Guid.NewGuid().ToString("D");

        var rowKey = Guid.NewGuid().ToString("D");

        var client = new TableServiceClient(fixture.Container.GetConnectionString());

        var tableClient = client.GetTableClient("Table" + Guid.NewGuid().ToString("N"));

        // When
        _ = await tableClient.CreateAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        _ = await tableClient.AddEntityAsync(new TableEntity(partitionKey, rowKey), TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var entityResponse = await tableClient.GetEntityAsync<TableEntity>(partitionKey, rowKey, cancellationToken: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(rowKey, entityResponse.Value.RowKey);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AzureService, "appconfig")]
    public async Task GetConfigurationReturnsSetValue()
    {
        // Given
        var key = Guid.NewGuid().ToString("D");
        var value = Guid.NewGuid().ToString("D");
        using var client = fixture.CreateHttpClient("appconfig");

        // When
        using var setResponse = await client.PutAsJsonAsync($"kv/{key}?api-version=2024-09-01", new { value }, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        await AssertSuccessAsync(setResponse).ConfigureAwait(true);

        using var getResponse = await client.GetAsync($"kv/{key}?api-version=2024-09-01", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        using var document = await ReadJsonAsync(getResponse).ConfigureAwait(true);

        // Then
        Assert.Equal(value, document.RootElement.GetProperty("value").GetString());
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AzureService, "cosmos")]
    public async Task GetDocumentReturnsCreatedDocument()
    {
        // Given
        var database = "db" + Guid.NewGuid().ToString("N");
        var documentId = Guid.NewGuid().ToString("D");
        using var client = fixture.CreateHttpClient("cosmos");

        // When
        using var databaseResponse = await client.PostAsJsonAsync("dbs", new { id = database }, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        await AssertSuccessAsync(databaseResponse).ConfigureAwait(true);

        using var collectionResponse = await client.PostAsJsonAsync($"dbs/{database}/colls", new { id = "items", partitionKey = new { paths = new[] { "/category" }, kind = "Hash" } }, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        await AssertSuccessAsync(collectionResponse).ConfigureAwait(true);

        using var createRequest = new HttpRequestMessage(HttpMethod.Post, $"dbs/{database}/colls/items/docs");
        createRequest.Headers.Add("x-ms-documentdb-partitionkey", "[\"test\"]");
        createRequest.Content = JsonContent.Create(new { id = documentId, category = "test", value = "created" });
        using var createResponse = await client.SendAsync(createRequest, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        await AssertSuccessAsync(createResponse).ConfigureAwait(true);

        using var getRequest = new HttpRequestMessage(HttpMethod.Get, $"dbs/{database}/colls/items/docs/{documentId}");
        getRequest.Headers.Add("x-ms-documentdb-partitionkey", "[\"test\"]");
        using var getResponse = await client.SendAsync(getRequest, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        using var document = await ReadJsonAsync(getResponse).ConfigureAwait(true);

        // Then
        Assert.Equal("created", document.RootElement.GetProperty("value").GetString());
        Assert.Contains(fixture.Container.GetServiceEndpoint("cosmos"), fixture.Container.GetCosmosConnectionString());
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AzureService, "keyvault")]
    public async Task GetSecretReturnsSetSecret()
    {
        // Given
        var secretName = "secret-" + Guid.NewGuid().ToString("N");
        var secretValue = Guid.NewGuid().ToString("D");
        using var client = fixture.CreateHttpClient("keyvault");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "fake");

        // When
        using var setResponse = await client.PutAsJsonAsync($"secrets/{secretName}?api-version=7.4", new { value = secretValue }, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        await AssertSuccessAsync(setResponse).ConfigureAwait(true);

        using var getResponse = await client.GetAsync($"secrets/{secretName}?api-version=7.4", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        using var document = await ReadJsonAsync(getResponse).ConfigureAwait(true);

        // Then
        Assert.Equal(secretValue, document.RootElement.GetProperty("value").GetString());
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AzureService, "functions")]
    public async Task GetFunctionAppReturnsCreatedApp()
    {
        // Given
        var appName = "app-" + Guid.NewGuid().ToString("N");
        using var client = fixture.CreateHttpClient("functions");

        // When
        using var createResponse = await client.PutAsJsonAsync($"admin/apps/{appName}", new { runtime = "dotnet" }, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        await AssertSuccessAsync(createResponse).ConfigureAwait(true);

        using var getResponse = await client.GetAsync($"admin/apps/{appName}", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        using var document = await ReadJsonAsync(getResponse).ConfigureAwait(true);

        // Then
        Assert.Equal(appName, document.RootElement.GetProperty("name").GetString());
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AzureService, "servicebus")]
    public async Task GetServiceBusNamespaceReturnsCreatedNamespace()
    {
        // Given
        var namespaceName = "sb-" + Guid.NewGuid().ToString("N");
        using var client = fixture.CreateHttpClient("servicebus");

        // When
        using var createResponse = await client.PutAsJsonAsync($"namespaces/{namespaceName}", new { }, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        await AssertSuccessAsync(createResponse).ConfigureAwait(true);

        using var getResponse = await client.GetAsync($"namespaces/{namespaceName}", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        using var document = await ReadJsonAsync(getResponse).ConfigureAwait(true);

        // Then
        Assert.Equal(namespaceName, document.RootElement.GetProperty("name").GetString());
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AzureService, "entra")]
    public async Task TokenEndpointReturnsAccessToken()
    {
        // Given
        using var client = fixture.CreateHttpClient();
        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_id", "11111111-1111-1111-1111-111111111111" },
            { "client_secret", "floci-az-dev-secret" },
            { "scope", "api://resource/.default" },
        });

        // When
        using var response = await client.PostAsync("00000000-0000-0000-0000-000000000002/oauth2/v2.0/token", content, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        using var document = await ReadJsonAsync(response).ConfigureAwait(true);

        // Then
        Assert.False(string.IsNullOrEmpty(document.RootElement.GetProperty("access_token").GetString()));
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AzureService, "managedidentity")]
    public async Task ImdsReturnsAccessToken()
    {
        // Given
        using var client = fixture.CreateHttpClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "metadata/identity/oauth2/token?resource=https%3A%2F%2Fmanagement.azure.com%2F&api-version=2018-02-01");
        request.Headers.Add("Metadata", "true");

        // When
        using var response = await client.SendAsync(request, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        using var document = await ReadJsonAsync(response).ConfigureAwait(true);

        // Then
        Assert.Equal("Bearer", document.RootElement.GetProperty("token_type").GetString());
        Assert.False(string.IsNullOrEmpty(document.RootElement.GetProperty("access_token").GetString()));
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AzureService, "email")]
    public async Task InspectionMailboxReturnsSentEmail()
    {
        // Given
        var subject = Guid.NewGuid().ToString("D");
        using var client = fixture.CreateHttpClient();
        var message = new
        {
            senderAddress = "DoNotReply@example.com",
            content = new { subject, plainText = "Hello from Testcontainers" },
            recipients = new { to = new[] { new { address = "dev@example.com" } } },
        };

        // When
        using var sendResponse = await client.PostAsJsonAsync("./emails:send?api-version=2023-03-31", message, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        await AssertSuccessAsync(sendResponse).ConfigureAwait(true);

        using var mailboxResponse = await client.GetAsync("emailMessages", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        using var document = await ReadJsonAsync(mailboxResponse).ConfigureAwait(true);

        // Then
        var email = Assert.Single(document.RootElement.GetProperty("value").EnumerateArray());
        Assert.Equal(subject, email.GetProperty("subject").GetString());
    }

    [Theory]
    [MemberData(nameof(ArmResources))]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AzureService, "arm")]
    public async Task ArmResourceSupportsLifecycle(string resourcePath, string payload, string expectedProperty, string expectedValue, bool supportsList)
    {
        // Given
        var resourceName = "tc" + Guid.NewGuid().ToString("N");
        var path = $"subscriptions/{FlociAzFixture.SubscriptionId}/resourceGroups/{FlociAzFixture.ResourceGroup}/providers/{resourcePath}"
            .Replace("{name}", resourceName, StringComparison.Ordinal);
        var queryIndex = resourcePath.IndexOf('?', StringComparison.Ordinal);
        var resourceIndex = resourcePath.IndexOf("/{name}", StringComparison.Ordinal);
        var collectionResourcePath = resourcePath.Substring(0, resourceIndex) + resourcePath.Substring(queryIndex);
        var collectionPath = $"subscriptions/{FlociAzFixture.SubscriptionId}/resourceGroups/{FlociAzFixture.ResourceGroup}/providers/{collectionResourcePath}";
        using var client = fixture.CreateHttpClient();

        // When
        using var content = new StringContent(payload.Replace("{name}", resourceName, StringComparison.Ordinal), Encoding.UTF8, "application/json");
        using var createResponse = await client.PutAsync(path, content, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        await AssertSuccessAsync(createResponse).ConfigureAwait(true);

        using var getResponse = await client.GetAsync(path, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        using var document = await ReadJsonAsync(getResponse).ConfigureAwait(true);

        // Then
        Assert.Equal(resourceName, document.RootElement.GetProperty("name").GetString());
        Assert.Equal(expectedValue.Replace("{name}", resourceName, StringComparison.Ordinal), GetProperty(document.RootElement, expectedProperty).GetString());

        if (supportsList)
        {
            using var listResponse = await client.GetAsync(collectionPath, TestContext.Current.CancellationToken)
                .ConfigureAwait(true);
            using var listDocument = await ReadJsonAsync(listResponse).ConfigureAwait(true);
            Assert.Contains(listDocument.RootElement.GetProperty("value").EnumerateArray(), resource => resource.GetProperty("name").GetString() == resourceName);
        }

        using var deleteResponse = await client.DeleteAsync(path, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        await AssertSuccessAsync(deleteResponse).ConfigureAwait(true);

        using var deletedResponse = await client.GetAsync(path, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        Assert.Equal(HttpStatusCode.NotFound, deletedResponse.StatusCode);
    }

    private static JsonElement GetProperty(JsonElement element, string path)
    {
        foreach (var segment in path.Split('.'))
        {
            element = element.GetProperty(segment);
        }

        return element;
    }

    private static async Task AssertSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        Assert.Fail($"{response.RequestMessage?.Method} {response.RequestMessage?.RequestUri} returned {(int)response.StatusCode}: {content}");
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        await AssertSuccessAsync(response).ConfigureAwait(true);
        return await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken).ConfigureAwait(true), cancellationToken: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
    }

    public sealed class FlociAzFixture : IAsyncLifetime
    {
        public const string SubscriptionId = "00000000-0000-0000-0000-000000000001";

        public const string ResourceGroup = "testcontainers";

        public FlociAzContainer Container { get; }
            = new FlociAzBuilder(TestSession.GetImageFromDockerfile())
                .WithEnvironment("FLOCI_AZ_SERVICES_EVENT_HUB_ENABLED", "true")
                .WithEnvironment("FLOCI_AZ_SERVICES_EVENT_HUB_MOCKED", "true")
                .WithEnvironment("FLOCI_AZ_SERVICES_FUNCTIONS_MOCKED", "true")
                .WithEnvironment("FLOCI_AZ_SERVICES_POSTGRES_MOCKED", "true")
                .WithEnvironment("FLOCI_AZ_SERVICES_AKS_MOCKED", "true")
                .WithEnvironment("FLOCI_AZ_SERVICES_ACR_MOCKED", "true")
                .WithEnvironment("FLOCI_AZ_SERVICES_REDIS_MOCKED", "true")
                .Build();

        public async ValueTask InitializeAsync()
        {
            await Container.StartAsync()
                .ConfigureAwait(false);

            using var client = CreateHttpClient();
            using var response = await client.PutAsJsonAsync($"subscriptions/{SubscriptionId}/resourceGroups/{ResourceGroup}?api-version=2021-04-01", new { location = "eastus" }, TestContext.Current.CancellationToken)
                .ConfigureAwait(false);
            await AssertSuccessAsync(response).ConfigureAwait(false);
        }

        public ValueTask DisposeAsync()
        {
            return Container.DisposeAsync();
        }

        public HttpClient CreateHttpClient(string service = null)
        {
            var endpoint = service is null ? Container.GetEndpoint() : Container.GetServiceEndpoint(service);
            return new HttpClient { BaseAddress = new Uri(endpoint) };
        }
    }
}
