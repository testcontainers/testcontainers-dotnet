namespace TestContainers.Minio;

public sealed class MinioContainer: DockerContainer
{
    private readonly MinioConfiguration _configuration;
    public MinioContainer(MinioConfiguration configuration, ILogger logger) : base(configuration, logger)
    {
        _configuration = configuration;
    }


    public string GetUserName()
    {
        return _configuration.UserName;
    }
    
    public string GetPassword()
    {
        return _configuration.Password;
    }
    
    public string GetAccessId()
    {
        return _configuration.UserName;
    }
    
    public string GetAccessKey()
    {
        return _configuration.Password;
    }
    
    public string GetMinioUrl()
    {
        var port = GetMappedPublicPort(MinioBuilder.MinioPort);
        return $"http://{Hostname}:{port}";
    }
}