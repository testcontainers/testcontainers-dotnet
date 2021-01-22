namespace DotNet.Testcontainers.Tests.Unit
{
  public readonly struct DockerResource
  {
    public static readonly DockerResource Container = new DockerResource("container");

    public static readonly DockerResource Image = new DockerResource("image");

    public static readonly DockerResource Network = new DockerResource("network");

    public static readonly DockerResource Volume = new DockerResource("volume");

    private DockerResource(string type)
    {
      this.Type = type;
    }

    public string Type { get; }
  }
}
