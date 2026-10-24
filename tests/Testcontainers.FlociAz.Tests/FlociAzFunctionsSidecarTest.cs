namespace Testcontainers.FlociAz;

[Collection(nameof(FlociAzSidecarCollection))]
public sealed class FlociAzFunctionsSidecarTest(FlociAzFunctionsSidecarTest.FlociAzFunctionsFixture fixture)
    : IClassFixture<FlociAzFunctionsSidecarTest.FlociAzFunctionsFixture>
{
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait("Service", "functions-real")]
    public async Task FunctionSidecarExecutesDeployedCode()
    {
        // Given
        var appName = "app" + Guid.NewGuid().ToString("N");
        const string functionName = "HttpTrigger";
        using var client = new HttpClient { BaseAddress = new Uri(fixture.Container.GetServiceEndpoint("functions")) };
        using var package = CreateFunctionPackage();

        // When
        using var appResponse = await client.PutAsJsonAsync($"admin/apps/{appName}", new { runtime = "node" }, TestContext.Current.CancellationToken).ConfigureAwait(true);
        await AssertSuccessAsync(appResponse).ConfigureAwait(true);
        using var deployResponse = await client.PutAsJsonAsync($"admin/apps/{appName}/functions/{functionName}", new { handler = "index.handler", zipBase64 = Convert.ToBase64String(package.ToArray()) }, TestContext.Current.CancellationToken).ConfigureAwait(true);
        await AssertSuccessAsync(deployResponse).ConfigureAwait(true);
        using var invokeResponse = await InvokeWhenReadyAsync(client, appName, functionName).ConfigureAwait(true);
        var responseBody = await invokeResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken).ConfigureAwait(true);

        // Then
        await AssertSuccessAsync(invokeResponse).ConfigureAwait(true);
        Assert.Contains("compatible", responseBody, StringComparison.OrdinalIgnoreCase);

        using var deleteResponse = await client.DeleteAsync($"admin/apps/{appName}", TestContext.Current.CancellationToken).ConfigureAwait(true);
        await AssertSuccessAsync(deleteResponse).ConfigureAwait(true);
    }

    private static MemoryStream CreateFunctionPackage()
    {
        var stream = new MemoryStream();
        using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, true))
        {
            WriteEntry(archive, "function.json", "{\"scriptFile\":\"index.js\",\"bindings\":[{\"authLevel\":\"anonymous\",\"type\":\"httpTrigger\",\"direction\":\"in\",\"name\":\"req\",\"methods\":[\"get\",\"post\"]},{\"type\":\"http\",\"direction\":\"out\",\"name\":\"res\"}]}");
            WriteEntry(archive, "index.js", "module.exports = async function (context) { context.res = { status: 200, body: 'compatible' }; }; ");
        }

        stream.Position = 0;
        return stream;
    }

    private static void WriteEntry(ZipArchive archive, string path, string content)
    {
        using var writer = new StreamWriter(archive.CreateEntry(path).Open(), Encoding.UTF8);
        writer.Write(content);
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

    private static async Task<HttpResponseMessage> InvokeWhenReadyAsync(HttpClient client, string appName, string functionName)
    {
        var timeout = DateTime.UtcNow.AddMinutes(2);
        HttpResponseMessage response;

        do
        {
            response = await client.PostAsJsonAsync($"api/{appName}/{functionName}", new { }, TestContext.Current.CancellationToken).ConfigureAwait(true);
            if (response.StatusCode != HttpStatusCode.NotFound)
            {
                return response;
            }

            response.Dispose();
            await Task.Delay(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken).ConfigureAwait(true);
        }
        while (DateTime.UtcNow < timeout);

        return await client.PostAsJsonAsync($"api/{appName}/{functionName}", new { }, TestContext.Current.CancellationToken).ConfigureAwait(true);
    }

    public sealed class FlociAzFunctionsFixture : IAsyncLifetime
    {
        public FlociAzContainer Container { get; }
            = new FlociAzBuilder(TestSession.GetImageFromDockerfile())
                .WithDockerSocket()
                .WithEnvironment("FLOCI_AZ_SERVICES_FUNCTIONS_MOCKED", "false")
                .Build();

        public async ValueTask InitializeAsync()
        {
            await Container.StartAsync().ConfigureAwait(false);
        }

        public ValueTask DisposeAsync()
        {
            return Container.DisposeAsync();
        }
    }
}
