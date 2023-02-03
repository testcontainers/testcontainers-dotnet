namespace Testcontainers.MsSql;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class MsSqlContainer : DockerContainer
{
    private readonly MsSqlConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="MsSqlContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public MsSqlContainer(MsSqlConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the MsSql connection string.
    /// </summary>
    /// <returns>The MsSql connection string.</returns>
    public string GetConnectionString()
    {
        var properties = new Dictionary<string, string>();
        properties.Add("Server", Hostname + "," + GetMappedPublicPort(MsSqlBuilder.MsSqlPort));
        properties.Add("Database", _configuration.Database);
        properties.Add("User Id", _configuration.Username);
        properties.Add("Password", _configuration.Password);
        properties.Add("TrustServerCertificate", bool.TrueString);
        return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
    }
}