namespace DotNet.Testcontainers.Core.Builder
{
  using System.Collections.Generic;
  using System.Linq;
  using Docker.DotNet.Models;

  /// <summary>
  /// Maps the user configuration of Testcontainers to the official Docker API container configuration.
  /// </summary>
  internal class DockerContainerConfig : InternalContainerConfig
  {
    public new IDictionary<string, EmptyStruct> ExposedPorts
    {
      get
      {
        return base.ExposedPorts.ToDictionary(exposedPort => $"{exposedPort.Key}/tcp", exposedPort => default(EmptyStruct));
      }
    }

    public new IList<string> Command
    {
      get
      {
        return base.Command.ToList();
      }
    }
  }
}
