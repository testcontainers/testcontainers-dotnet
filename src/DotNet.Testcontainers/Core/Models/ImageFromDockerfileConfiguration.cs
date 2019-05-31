namespace DotNet.Testcontainers.Core.Models
{
  using System;

  internal class ImageFromDockerfileConfiguration
  {
    public string Image { get; set; } = Guid.NewGuid().ToString("n").Substring(0, 12);

    public string DockerfileDirectory { get; set; } = ".";

    public bool DeleteIfExists { get; set; } = true;
  }
}
