namespace DotNet.Testcontainers.Containers.OutputConsumers
{
  using System;
  using System.IO;

  /// <summary>
  /// Receives the output of the Testcontainer.
  /// </summary>
  public interface IOutputConsumer : IDisposable
  {
    /// <summary>
    /// Receives Stdout.
    /// </summary>
    Stream Stdout { get; }

    /// <summary>
    /// Receives Stderr.
    /// </summary>
    Stream Stderr { get; }
  }
}
