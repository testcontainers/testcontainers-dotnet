namespace Testcontainers.Floci;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class FlociContainer : DockerContainer
{
    /// <summary>The Floci Docker image.</summary>
    public const string FlociImage = "floci/floci:latest";

    /// <summary>The port Floci listens on inside the container.</summary>
    public const ushort FlociPort = 4566;

    /// <summary>The default AWS access key used by Floci.</summary>
    public const string AccessKey = "test";

    /// <summary>The default AWS secret key used by Floci.</summary>
    public const string SecretKey = "test";

    /// <summary>The default AWS region used by Floci.</summary>
    public const string Region = "us-east-1";

    /// <summary>
    /// Initializes a new instance of the <see cref="FlociContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public FlociContainer(FlociConfiguration configuration)
        : base(configuration)
    {
    }

    /// <summary>Returns the HTTP base URL for connecting to Floci.</summary>
    public string GetConnectionString() =>
        new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(FlociPort)).ToString();

    /// <summary>Returns the AWS access key.</summary>
    public string GetAccessKey() => AccessKey;

    /// <summary>Returns the AWS secret key.</summary>
    public string GetSecretKey() => SecretKey;

    /// <summary>Returns the AWS region.</summary>
    public string GetRegion() => Region;
}
