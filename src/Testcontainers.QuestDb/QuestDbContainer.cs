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
    /// Gets the PostgreSQL connection string for SQL queries.
    /// </summary>
    /// <returns>The PostgreSQL wire protocol connection string.</returns>
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
    /// Gets the REST API base address.
    /// </summary>
    /// <returns>The REST API base address.</returns>
    public string GetRestApiAddress()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(QuestDbBuilder.QuestDbHttpPort)).ToString();
    }

    /// <summary>
    /// Gets the HTTP API port.
    /// </summary>
    /// <returns>The mapped HTTP API port.</returns>
    public int GetHttpApiPort()
    {
        return GetMappedPublicPort(QuestDbBuilder.QuestDbHttpPort);
    }

    /// <summary>
    /// Gets the Web Console URL.
    /// </summary>
    /// <returns>The Web Console URL.</returns>
    public string GetWebConsoleUrl()
    {
        return GetRestApiAddress();
    }

    /// <summary>
    /// Gets the InfluxDB Line Protocol (ILP) host.
    /// </summary>
    /// <returns>The ILP host.</returns>
    public string GetInfluxLineProtocolHost()
    {
        return Hostname;
    }

    /// <summary>
    /// Gets the InfluxDB Line Protocol (ILP) port.
    /// </summary>
    /// <returns>The ILP port.</returns>
    public int GetInfluxLineProtocolPort()
    {
        return GetMappedPublicPort(QuestDbBuilder.QuestDbInfluxLinePort);
    }
}
