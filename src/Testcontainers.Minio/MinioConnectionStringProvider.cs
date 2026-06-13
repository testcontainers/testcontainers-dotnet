namespace Testcontainers.Minio;

/// <summary>
/// Provides the Minio connection string.
/// </summary>
internal sealed class MinioConnectionStringProvider : ContainerConnectionStringProvider<MinioContainer, MinioConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}