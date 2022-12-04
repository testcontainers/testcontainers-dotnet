namespace Testcontainers.Common
{
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;

  [PublicAPI]
  public static class CommonImages
  {
    public static readonly IDockerImage Ryuk = new DockerImage("testcontainers/ryuk:0.3.4");

    public static readonly IDockerImage Alpine = new DockerImage("alpine:3.17");

    public static readonly IDockerImage Nginx = new DockerImage("nginx:1.22");
  }
}
