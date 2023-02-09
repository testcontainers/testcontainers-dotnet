namespace Testcontainers.Oracle;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class OracleContainer : DockerContainer
{
    private readonly OracleConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="OracleContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public OracleContainer(OracleConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the Oracle connection string.
    /// </summary>
    /// <returns>The Oracle connection string.</returns>
    public string GetConnectionString()
    {
        const string dataSource = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));User Id={3};Password={4};";
        return string.Format(dataSource, Hostname, GetMappedPublicPort(OracleBuilder.OraclePort), _configuration.Database, _configuration.Username, _configuration.Password);
    }
}