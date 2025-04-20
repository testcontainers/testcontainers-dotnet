namespace Testcontainers.Qdrant;

public sealed class QdrantSecureContainerTest : IAsyncLifetime
{
    private static readonly string ApiKey = Guid.NewGuid().ToString("D");

    private static readonly string CommonName = PemCertificate.Instance.CommonName;

    private static readonly string Certificate = PemCertificate.Instance.Certificate;

    private static readonly string CertificateKey = PemCertificate.Instance.CertificateKey;

    private static readonly string Thumbprint = PemCertificate.Instance.Thumbprint;

    private readonly QdrantContainer _qdrantContainer = new QdrantBuilder()
        // # --8<-- [start:ConfigureQdrantContainerApiKey]
        .WithApiKey(ApiKey)
        // # --8<-- [end:ConfigureQdrantContainerApiKey]

        // # --8<-- [start:ConfigureQdrantContainerCertificate]
        .WithCertificate(Certificate, CertificateKey)
        // # --8<-- [end:ConfigureQdrantContainerCertificate]
        .Build();

    public async ValueTask InitializeAsync()
    {
        await _qdrantContainer.StartAsync();
    }

    public ValueTask DisposeAsync()
    {
        return _qdrantContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task HealthReturnsValidResponse()
    {
        // Given
        // # --8<-- [start:ConfigureQdrantClientCertificate-1]
        using var httpMessageHandler = new HttpClientHandler();
        httpMessageHandler.ServerCertificateCustomValidationCallback = CertificateValidation.Thumbprint(Thumbprint);

        using var httpClient = new HttpClient(httpMessageHandler);
        httpClient.DefaultRequestHeaders.Host = CommonName;
        // # --8<-- [end:ConfigureQdrantClientCertificate-1]

        // # --8<-- [start:ConfigureQdrantClientApiKey]
        httpClient.DefaultRequestHeaders.Add("api-key", ApiKey);
        // # --8<-- [end:ConfigureQdrantClientApiKey]

        // # --8<-- [start:ConfigureQdrantClientCertificate-2]
        var grpcChannelOptions = new GrpcChannelOptions();
        grpcChannelOptions.HttpClient = httpClient;

        using var grpcChannel = GrpcChannel.ForAddress(_qdrantContainer.GetGrpcConnectionString(), grpcChannelOptions);

        using var grpcClient = new QdrantGrpcClient(grpcChannel);

        using var client = new QdrantClient(grpcClient);
        // # --8<-- [end:ConfigureQdrantClientCertificate-2]

        // When
        var response = await client.HealthAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.NotEmpty(response.Title);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task HealthWithoutApiKeyReturnsUnauthenticated()
    {
        // Given
        using var httpMessageHandler = new HttpClientHandler();
        httpMessageHandler.ServerCertificateCustomValidationCallback = CertificateValidation.Thumbprint(Thumbprint);

        using var httpClient = new HttpClient(httpMessageHandler);
        httpClient.DefaultRequestHeaders.Host = CommonName;

        var grpcChannelOptions = new GrpcChannelOptions();
        grpcChannelOptions.HttpClient = httpClient;

        using var grpcChannel = GrpcChannel.ForAddress(_qdrantContainer.GetGrpcConnectionString(), grpcChannelOptions);

        using var grpcClient = new QdrantGrpcClient(grpcChannel);

        using var client = new QdrantClient(grpcClient);

        // When
        var exception = await Assert.ThrowsAsync<RpcException>(() => client.HealthAsync(TestContext.Current.CancellationToken))
            .ConfigureAwait(true);

        // Then
        Assert.Equal(StatusCode.Unauthenticated, exception.Status.StatusCode);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task HealthWithoutCertificateValidationReturnsSecureConnectionError()
    {
        // Given
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(_qdrantContainer.GetHttpConnectionString());
        httpClient.DefaultRequestHeaders.Host = CommonName;
        httpClient.DefaultRequestHeaders.Add("api-key", ApiKey);

        // When
        var exception = await Assert.ThrowsAsync<HttpRequestException>(() => httpClient.GetAsync("/", TestContext.Current.CancellationToken))
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpRequestError.SecureConnectionError, exception.HttpRequestError);
    }
}