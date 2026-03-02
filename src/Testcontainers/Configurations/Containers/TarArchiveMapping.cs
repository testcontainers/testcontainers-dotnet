using System.IO;

namespace DotNet.Testcontainers.Configurations.Containers
{
  public sealed record TarArchiveMapping
  {
    public TarArchiveMapping(Stream tarArchive, string containerPath)
    {
      TarArchive = tarArchive;
      ContainerPath = containerPath;
    }

    public Stream TarArchive { get; }

    public string ContainerPath { get; }
  }
}
