namespace DotNet.Testcontainers.Commons
{
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;

  [PublicAPI]
  public static class CommonImages
  {
    public static readonly IImage Ryuk = new DockerImage("testcontainers/ryuk:0.3.4");

    public static readonly IImage Alpine = new DockerImage("alpine:3.17");

    public static readonly IImage Nginx = new DockerImage("nginx:1.22");
  }
}
