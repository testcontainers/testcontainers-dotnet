namespace Testcontainers.FlociAz;

public sealed partial class FlociAzContainerTest
{
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AzureService, "eventhub")]
    public async Task MockedEventHubSupportsNamespaceManagement()
    {
        // Given
        var namespaceName = "eventhub-" + Guid.NewGuid().ToString("N");
        using var client = fixture.CreateHttpClient("eventhub");

        // When
        using var createResponse = await client.PutAsJsonAsync($"namespaces/{namespaceName}", new { entities = "events:2", consumerGroups = "$Default,test" }, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        await AssertSuccessAsync(createResponse).ConfigureAwait(true);

        using var listResponse = await client.GetAsync("namespaces", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        using var namespaces = await ReadJsonAsync(listResponse).ConfigureAwait(true);

        // Then
        Assert.Contains(namespaces.RootElement.GetProperty("namespaces").EnumerateArray(), item => item.GetProperty("name").GetString() == namespaceName);

        using var deleteResponse = await client.DeleteAsync($"namespaces/{namespaceName}", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        await AssertSuccessAsync(deleteResponse).ConfigureAwait(true);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AzureService, "graph")]
    public async Task GraphReturnsSeededDirectoryMembership()
    {
        // Given
        using var client = fixture.CreateHttpClient();

        // When
        using var groupsResponse = await client.PostAsJsonAsync("v1.0/users/dev-user@floci-az.local/getMemberGroups", new { securityEnabledOnly = true }, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        using var groups = await ReadJsonAsync(groupsResponse).ConfigureAwait(true);

        using var principalsResponse = await client.GetAsync("v1.0/servicePrincipals?$filter=appId%20eq%20'11111111-1111-1111-1111-111111111111'", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        using var principals = await ReadJsonAsync(principalsResponse).ConfigureAwait(true);

        // Then
        Assert.Contains(groups.RootElement.GetProperty("value").EnumerateArray(), group => group.GetString() == "44444444-4444-4444-4444-444444444444");
        Assert.NotEmpty(principals.RootElement.GetProperty("value").EnumerateArray());
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AzureService, "functions")]
    public async Task MockedFunctionSupportsDeploymentInvocationAndDeletion()
    {
        // Given
        var appName = "app-" + Guid.NewGuid().ToString("N");
        var functionName = "function-" + Guid.NewGuid().ToString("N");
        using var client = fixture.CreateHttpClient("functions");
        using var package = CreateFunctionPackage(functionName);

        // When
        using var appResponse = await client.PutAsJsonAsync($"admin/apps/{appName}", new { runtime = "node" }, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        await AssertSuccessAsync(appResponse).ConfigureAwait(true);

        using var deployResponse = await client.PutAsJsonAsync($"admin/apps/{appName}/functions/{functionName}", new { handler = "index.handler", zipBase64 = Convert.ToBase64String(package.ToArray()) }, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        await AssertSuccessAsync(deployResponse).ConfigureAwait(true);

        using var listResponse = await client.GetAsync($"admin/apps/{appName}/functions", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        using var functions = await ReadJsonAsync(listResponse).ConfigureAwait(true);

        using var invocationResponse = await client.PostAsJsonAsync($"api/{appName}/{functionName}", new { value = "test" }, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        await AssertSuccessAsync(invocationResponse).ConfigureAwait(true);

        using var deleteResponse = await client.DeleteAsync($"admin/apps/{appName}/functions/{functionName}", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        await AssertSuccessAsync(deleteResponse).ConfigureAwait(true);

        // Then
        Assert.Contains(functions.RootElement.GetProperty("value").EnumerateArray(), function => function.GetProperty("name").GetString() == functionName);
        using var deletedResponse = await client.GetAsync($"admin/apps/{appName}/functions/{functionName}", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        Assert.Equal(HttpStatusCode.NotFound, deletedResponse.StatusCode);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AzureService, "servicebus")]
    public async Task MockedServiceBusSupportsQueueTopicSubscriptionAndRuleTopology()
    {
        // Given
        var queueName = "queue-" + Guid.NewGuid().ToString("N");
        var topicName = "topic-" + Guid.NewGuid().ToString("N");
        var subscriptionName = "subscription-" + Guid.NewGuid().ToString("N");
        var ruleName = "rule-" + Guid.NewGuid().ToString("N");
        using var client = fixture.CreateHttpClient("servicebus");

        // When
        using var queueResponse = await PutXmlAsync(client, queueName, "<QueueDescription xmlns=\"http://schemas.microsoft.com/netservices/2010/10/servicebus/connect\" />").ConfigureAwait(true);
        await AssertSuccessAsync(queueResponse).ConfigureAwait(true);

        using var topicResponse = await PutXmlAsync(client, topicName, "<TopicDescription xmlns=\"http://schemas.microsoft.com/netservices/2010/10/servicebus/connect\" />").ConfigureAwait(true);
        await AssertSuccessAsync(topicResponse).ConfigureAwait(true);

        using var subscriptionResponse = await PutXmlAsync(client, $"{topicName}/subscriptions/{subscriptionName}", "<SubscriptionDescription xmlns=\"http://schemas.microsoft.com/netservices/2010/10/servicebus/connect\" />").ConfigureAwait(true);
        await AssertSuccessAsync(subscriptionResponse).ConfigureAwait(true);

        const string rule = "<RuleDescription xmlns=\"http://schemas.microsoft.com/netservices/2010/10/servicebus/connect\"><Filter xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\" i:type=\"TrueFilter\" /></RuleDescription>";
        using var ruleResponse = await PutXmlAsync(client, $"{topicName}/subscriptions/{subscriptionName}/rules/{ruleName}", rule).ConfigureAwait(true);
        await AssertSuccessAsync(ruleResponse).ConfigureAwait(true);

        using var queuesResponse = await client.GetAsync("$Resources/queues", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        await AssertSuccessAsync(queuesResponse).ConfigureAwait(true);
        var queues = await queuesResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken).ConfigureAwait(true);

        using var rulesResponse = await client.GetAsync($"{topicName}/subscriptions/{subscriptionName}/rules", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        await AssertSuccessAsync(rulesResponse).ConfigureAwait(true);
        var rules = await rulesResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken).ConfigureAwait(true);

        // Then
        Assert.Contains(queueName, queues, StringComparison.Ordinal);
        Assert.Contains(ruleName, rules, StringComparison.Ordinal);

        using var deleteRuleResponse = await client.DeleteAsync($"{topicName}/subscriptions/{subscriptionName}/rules/{ruleName}", TestContext.Current.CancellationToken).ConfigureAwait(true);
        await AssertSuccessAsync(deleteRuleResponse).ConfigureAwait(true);
        using var deleteSubscriptionResponse = await client.DeleteAsync($"{topicName}/subscriptions/{subscriptionName}", TestContext.Current.CancellationToken).ConfigureAwait(true);
        await AssertSuccessAsync(deleteSubscriptionResponse).ConfigureAwait(true);
        using var deleteTopicResponse = await client.DeleteAsync(topicName, TestContext.Current.CancellationToken).ConfigureAwait(true);
        await AssertSuccessAsync(deleteTopicResponse).ConfigureAwait(true);
        using var deleteQueueResponse = await client.DeleteAsync(queueName, TestContext.Current.CancellationToken).ConfigureAwait(true);
        await AssertSuccessAsync(deleteQueueResponse).ConfigureAwait(true);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AzureService, "monitor")]
    public async Task MonitorIngestsAndQueriesCustomLogs()
    {
        // Given
        var workspaceName = "ws" + Guid.NewGuid().ToString("N");
        var endpointName = "dce" + Guid.NewGuid().ToString("N");
        var ruleName = "dcr" + Guid.NewGuid().ToString("N");
        var providers = $"subscriptions/{FlociAzFixture.SubscriptionId}/resourceGroups/{FlociAzFixture.ResourceGroup}/providers";
        var workspacePath = $"{providers}/Microsoft.OperationalInsights/workspaces/{workspaceName}?api-version=2023-09-01";
        var endpointPath = $"{providers}/Microsoft.Insights/dataCollectionEndpoints/{endpointName}?api-version=2023-09-01";
        var rulePath = $"{providers}/Microsoft.Insights/dataCollectionRules/{ruleName}?api-version=2023-09-01";
        var workspaceId = $"/{providers}/Microsoft.OperationalInsights/workspaces/{workspaceName}";
        using var client = fixture.CreateHttpClient();

        // When
        using var workspaceResponse = await client.PutAsJsonAsync(workspacePath, new { location = "eastus", properties = new { } }, TestContext.Current.CancellationToken).ConfigureAwait(true);
        using var workspace = await ReadJsonAsync(workspaceResponse).ConfigureAwait(true);
        var customerId = workspace.RootElement.GetProperty("properties").GetProperty("customerId").GetString();

        using var endpointResponse = await client.PutAsJsonAsync(endpointPath, new { location = "eastus", properties = new { } }, TestContext.Current.CancellationToken).ConfigureAwait(true);
        await AssertSuccessAsync(endpointResponse).ConfigureAwait(true);

        var rulePayload = new
        {
            location = "eastus",
            properties = new
            {
                destinations = new { logAnalytics = new[] { new { name = "workspace", workspaceResourceId = workspaceId } } },
            },
        };
        using var ruleResponse = await client.PutAsJsonAsync(rulePath, rulePayload, TestContext.Current.CancellationToken).ConfigureAwait(true);
        using var ruleDocument = await ReadJsonAsync(ruleResponse).ConfigureAwait(true);
        var immutableId = ruleDocument.RootElement.GetProperty("properties").GetProperty("immutableId").GetString();

        var records = new[] { new { TimeGenerated = "2026-09-01T12:00:00Z", Level = "ERROR", Message = "compatible" } };
        using var ingestResponse = await client.PostAsJsonAsync($"dataCollectionRules/{immutableId}/streams/Custom-Test_CL?api-version=2023-01-01", records, TestContext.Current.CancellationToken).ConfigureAwait(true);
        await AssertSuccessAsync(ingestResponse).ConfigureAwait(true);

        using var queryResponse = await client.PostAsJsonAsync($"v1/workspaces/{customerId}/query", new { query = "Test_CL | where level == 'ERROR' | project message | take 1" }, TestContext.Current.CancellationToken).ConfigureAwait(true);
        using var query = await ReadJsonAsync(queryResponse).ConfigureAwait(true);

        // Then
        var table = Assert.Single(query.RootElement.GetProperty("tables").EnumerateArray());
        var row = Assert.Single(table.GetProperty("rows").EnumerateArray());
        Assert.Equal("compatible", row[0].GetString());

        using var deleteRuleResponse = await client.DeleteAsync(rulePath, TestContext.Current.CancellationToken).ConfigureAwait(true);
        await AssertSuccessAsync(deleteRuleResponse).ConfigureAwait(true);
        using var deleteEndpointResponse = await client.DeleteAsync(endpointPath, TestContext.Current.CancellationToken).ConfigureAwait(true);
        await AssertSuccessAsync(deleteEndpointResponse).ConfigureAwait(true);
        using var deleteWorkspaceResponse = await client.DeleteAsync(workspacePath, TestContext.Current.CancellationToken).ConfigureAwait(true);
        await AssertSuccessAsync(deleteWorkspaceResponse).ConfigureAwait(true);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AzureService, "eventgrid")]
    public async Task EventGridReturnsKeysAndAcceptsPublishedEvents()
    {
        // Given
        var topicName = "topic" + Guid.NewGuid().ToString("N");
        var topicPath = $"subscriptions/{FlociAzFixture.SubscriptionId}/resourceGroups/{FlociAzFixture.ResourceGroup}/providers/Microsoft.EventGrid/topics/{topicName}?api-version=2025-02-15";
        using var client = fixture.CreateHttpClient();

        // When
        using var topicResponse = await client.PutAsJsonAsync(topicPath, new { location = "eastus", properties = new { inputSchema = "EventGridSchema" } }, TestContext.Current.CancellationToken).ConfigureAwait(true);
        await AssertSuccessAsync(topicResponse).ConfigureAwait(true);

        using var keysResponse = await client.PostAsJsonAsync(topicPath.Replace("?", "/listKeys?", StringComparison.Ordinal), new { }, TestContext.Current.CancellationToken).ConfigureAwait(true);
        using var keys = await ReadJsonAsync(keysResponse).ConfigureAwait(true);

        client.DefaultRequestHeaders.Add("aeg-sas-key", keys.RootElement.GetProperty("key1").GetString());
        var events = new[] { new { id = Guid.NewGuid().ToString("D"), subject = "/test/1", eventType = "Test.Created", eventTime = DateTimeOffset.UtcNow, data = new { value = 1 }, dataVersion = "1.0" } };
        using var publishResponse = await client.PostAsJsonAsync($"{topicName}-eventgrid/api/events", events, TestContext.Current.CancellationToken).ConfigureAwait(true);

        // Then
        await AssertSuccessAsync(publishResponse).ConfigureAwait(true);

        using var deleteResponse = await client.DeleteAsync(topicPath, TestContext.Current.CancellationToken).ConfigureAwait(true);
        await AssertSuccessAsync(deleteResponse).ConfigureAwait(true);
    }

    private static async Task<HttpResponseMessage> PutXmlAsync(HttpClient client, string path, string xml)
    {
        return await client.PutAsync(path, new StringContent(xml, Encoding.UTF8, "application/atom+xml"), TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
    }

    private static MemoryStream CreateFunctionPackage(string functionName)
    {
        var stream = new MemoryStream();
        using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, true))
        {
            WriteEntry(archive, "host.json", "{\"version\":\"2.0\"}");
            WriteEntry(archive, $"{functionName}/function.json", "{\"bindings\":[{\"authLevel\":\"anonymous\",\"type\":\"httpTrigger\",\"direction\":\"in\",\"name\":\"req\"},{\"type\":\"http\",\"direction\":\"out\",\"name\":\"res\"}]}");
            WriteEntry(archive, $"{functionName}/index.js", "module.exports = async () => ({ status: 200, body: 'compatible' });");
        }

        stream.Position = 0;
        return stream;
    }

    private static void WriteEntry(ZipArchive archive, string path, string content)
    {
        using var writer = new StreamWriter(archive.CreateEntry(path).Open(), Encoding.UTF8);
        writer.Write(content);
    }
}
