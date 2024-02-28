namespace Testcontainers.Milvus;

public class MilvusContainer(MilvusConfiguration configuration, ILogger logger)
    : DockerContainer(configuration, logger)
{
    /// <summary>
    /// Gets the Milvus endpoint.
    /// </summary>
    /// <returns>The Milvus endpoint.</returns>
    public Uri GetEndpoint()
        => new Uri($"http://{Hostname}:{GetMappedPublicPort(MilvusBuilder.MilvusGrpcPort)}");
}
