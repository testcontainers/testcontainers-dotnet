namespace Testcontainers.Typesense;

public class TypesenseContainer : DockerContainer
{
    private readonly TypesenseConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypesenseContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public TypesenseContainer(TypesenseConfiguration configuration)
        : base(configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the blob endpoint
    /// </summary>
    /// <returns>The Azurite blob endpoint</returns>
    public TypesenseNode GetNode()
    {
        var mappedPort = GetMappedPublicPort(_configuration.Port);

        var baseAddress = new UriBuilder(Uri.UriSchemeHttp, Hostname, mappedPort).Uri;

        return new TypesenseNode()
        {
            BaseAddress = baseAddress,
            Protocol = Uri.UriSchemeHttp,
            Host = Hostname,
            Port = mappedPort.ToString(),
            ApiKey = _configuration.ApiKey
        };
    }
}
