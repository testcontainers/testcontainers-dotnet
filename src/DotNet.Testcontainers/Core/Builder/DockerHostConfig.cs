namespace DotNet.Testcontainers.Core.Builder
{
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using Docker.DotNet.Models;
  using static LanguageExt.Prelude;

  /// <summary>
  /// Maps the user configuration of Testcontainers to the official Docker API host configuration.
  /// </summary>
  internal class DockerHostConfig : InternalHostConfig
  {
    public new IDictionary<string, IList<PortBinding>> PortBindings
    {
      get
      {
        return base.PortBindings.ToDictionary(binding => $"{binding.Value}/tcp", binding => List(new PortBinding { HostPort = binding.Key }).ToList() as IList<PortBinding>);
      }
    }

    public new IList<Mount> Mounts
    {
      get
      {
        return base.Mounts.Select(mount => new Mount { Source = Path.GetFullPath(mount.Key), Target = mount.Value, Type = "bind" }).ToList();
      }
    }
  }
}
