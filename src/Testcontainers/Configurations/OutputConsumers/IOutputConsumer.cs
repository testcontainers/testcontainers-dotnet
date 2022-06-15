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
    /// Gets the stream that receives stdout.
    /// </summary>
    Stream Stdout { get; }

    /// <summary>
    /// Gets the stream that receives stderr.
    /// </summary>
    Stream Stderr { get; }
  }
}
