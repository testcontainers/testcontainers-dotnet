namespace Testcontainers.QuestDb;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class QuestDbContainer : DockerContainer, IDatabaseContainer
{
    private readonly QuestDbConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="QuestDbContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public QuestDbContainer(QuestDbConfiguration configuration)
        : base(configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the QuestDb connection string.
    /// </summary>
    /// <remarks>
    /// QuestDb exposes its SQL interface over the PostgreSQL wire protocol.
    /// </remarks>
    /// <returns>The QuestDb connection string.</returns>
    public string GetConnectionString()
    {
        var properties = new Dictionary<string, string>();
        properties.Add("Host", Hostname);
        properties.Add("Port", GetMappedPublicPort(QuestDbBuilder.QuestDbPgPort).ToString());
        properties.Add("Database", QuestDbBuilder.DefaultDatabase);
        properties.Add("Username", _configuration.Username);
        properties.Add("Password", _configuration.Password);
        properties.Add("Server Compatibility Mode", "NoTypeLoading");
        return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
    }

    /// <summary>
    /// Gets the QuestDb base address.
    /// </summary>
    /// <remarks>
    /// QuestDb serves the REST API and the Web Console on this address.
    /// </remarks>
    /// <returns>The QuestDb base address.</returns>
    public string GetBaseAddress()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(QuestDbBuilder.QuestDbHttpPort)).ToString();
    }

    /// <summary>
    /// Gets the QuestDb ILP address.
    /// </summary>
    /// <remarks>
    /// QuestDb ingests the InfluxDB Line Protocol (ILP) over TCP on this address.
    /// </remarks>
    /// <returns>The QuestDb ILP address.</returns>
    public string GetIlpAddress()
    {
        return new UriBuilder("tcp", Hostname, GetMappedPublicPort(QuestDbBuilder.QuestDbInfluxLinePort)).ToString();
    }
}