namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.IO;

  /// <summary>
  /// Receives the output of the Testcontainer.
  /// </summary>
  public interface IOutputConsumer : IDisposable
  {
    /// <summary>
    /// Gets a value indicating whether the <see cref="IOutputConsumer" /> is enabled or not.
    /// </summary>
    bool Enabled { get; }

    /// <summary>
    /// Gets the stream that receives stdout.
    /// </summary>
    Stream Stdout { get; }

    /// <summary>
    /// Gets the stream that receives stderr.
    /// </summary>
    Stream Stderr { get; }
  }
}
