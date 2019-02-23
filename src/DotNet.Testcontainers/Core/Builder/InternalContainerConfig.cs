namespace DotNet.Testcontainers.Core.Builder
{
  using System.Collections.Generic;

  /// <summary>
  /// Holds the container configuration for Testcontainers.
  /// </summary>
  internal class InternalContainerConfig
  {
    public string Image { get; private set; }

    public string Name { get; private set; }

    protected IReadOnlyDictionary<string, string> ExposedPorts { get; private set; }

    protected IReadOnlyCollection<string> Command { get; private set; }

    public void SetImage(string image)
    {
      this.Image = image;
    }

    public void SetName(string name)
    {
      this.Name = name;
    }

    public void SetExposedPorts(IReadOnlyDictionary<string, string> exposedPorts)
    {
      this.ExposedPorts = exposedPorts;
    }

    public void SetCommand(IReadOnlyCollection<string> command)
    {
      this.Command = command;
    }
  }
}
