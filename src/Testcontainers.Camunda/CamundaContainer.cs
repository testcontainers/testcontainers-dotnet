namespace Testcontainers.Camunda;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class CamundaContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CamundaContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public CamundaContainer(IContainerConfiguration configuration) : base(configuration)
    {
    }

    /// <summary>
    /// Gets the Camunda connection string.
    /// </summary>
    /// <returns>The Camunda connection string.</returns>
    public string GetConnectionString()
    {
        var endpoint = new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(CamundaBuilder.CamundaPort));
        endpoint.Path = "engine-rest";
        return endpoint.ToString();
    }
}