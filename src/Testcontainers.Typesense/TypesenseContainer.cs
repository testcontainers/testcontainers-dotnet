namespace Testcontainers.Typesense;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class TypesenseContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TypesenseContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public TypesenseContainer(TypesenseConfiguration configuration)
        : base(configuration)
    {
    }

    /// <summary>
    /// Gets the Typesense base address.
    /// </summary>
    /// <returns>The Typesense base address.</returns>
    public string GetBaseAddress()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(TypesenseBuilder.TypesensePort)).ToString();
    }
}