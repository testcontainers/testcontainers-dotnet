namespace DotNet.Testcontainers.Configurations
{
  using JetBrains.Annotations;

  /// <summary>
  /// This class represents a binding of a file or directory.
  /// </summary>
  public interface IBindMount : IMount
  {
    /// <summary>
    /// Gets the absolute path of a file or directory to mount on the host system.
    /// </summary>
    [NotNull]
    string HostPath { get; }
  }
}
