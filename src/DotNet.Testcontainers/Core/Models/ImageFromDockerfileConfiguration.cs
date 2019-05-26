namespace DotNet.Testcontainers.Core.Models
{
  using System;

  internal class ImageFromDockerfileConfiguration
  {
    public string Image { get; set; } = Guid.NewGuid().ToString("n");

    public string DockerfileDirectory { get; set; } = ".";

    public bool DeleteIfExits { get; set; } = true;
  }
}
