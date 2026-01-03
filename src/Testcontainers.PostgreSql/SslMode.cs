namespace Testcontainers.PostgreSql;

/// <summary>
/// Represents the SSL mode for PostgreSQL connections.
/// </summary>
public enum SslMode
{
    /// <summary>
    /// SSL is disabled.
    /// </summary>
    Disable,

    /// <summary>
    /// SSL is required.
    /// </summary>
    Require,

    /// <summary>
    /// SSL is required, and the server certificate is verified against the root certificate.
    /// </summary>
    VerifyCa,

    /// <summary>
    /// SSL is required, and the server certificate is verified against the root certificate and the common name.
    /// </summary>
    VerifyFull
}