namespace Testcontainers.SqlEdge;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class SqlEdgeContainer : DockerContainer, IDatabaseContainer
{
    private readonly SqlEdgeConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlEdgeContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public SqlEdgeContainer(SqlEdgeConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the SqlEdge connection string.
    /// </summary>
    /// <returns>The SqlEdge connection string.</returns>
    public string GetConnectionString()
    {
        var properties = new Dictionary<string, string>();
        properties.Add("Server", Hostname + "," + GetMappedPublicPort(SqlEdgeBuilder.SqlEdgePort));
        properties.Add("Database", _configuration.Database);
        properties.Add("User Id", _configuration.Username);
        properties.Add("Password", _configuration.Password);
        properties.Add("TrustServerCertificate", bool.TrueString);
        return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
    }
}