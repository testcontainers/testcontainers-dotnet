namespace Testcontainers.Qdrant;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public class QdrantContainer : DockerContainer
{
	private readonly QdrantConfiguration _configuration;

	public QdrantContainer(QdrantConfiguration configuration) : base(configuration)
	{
		_configuration = configuration;
	}
    
	public string GetHttpConnectionString()
	{
		var scheme = _configuration.Certificate != null ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;
		var endpoint = new UriBuilder(scheme, Hostname, GetMappedPublicPort(QdrantBuilder.QdrantHttpPort));
		return endpoint.ToString();
	}
	
	public string GetGrpcConnectionString()
	{
		var scheme = _configuration.Certificate != null ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;
		var endpoint = new UriBuilder(scheme, Hostname, GetMappedPublicPort(QdrantBuilder.QdrantGrpcPort));
		return endpoint.ToString();
	}
}
