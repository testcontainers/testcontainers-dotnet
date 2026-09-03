namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.IO;

  /// <inheritdoc cref="IOutputConsumer" />
  internal sealed class RedirectStdoutAndStderrToStream : IOutputConsumer
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="RedirectStdoutAndStderrToStream" /> class.
    /// </summary>
    public RedirectStdoutAndStderrToStream()
      : this(Console.OpenStandardOutput(), Console.OpenStandardError())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedirectStdoutAndStderrToStream" /> class.
    /// </summary>
    /// <param name="stdout">The stdout stream.</param>
    /// <param name="stderr">The stderr stream.</param>
    /// <exception cref="ArgumentException">Thrown when a stream is not writable.</exception>
    public RedirectStdoutAndStderrToStream(Stream stdout, Stream stderr)
    {
      if (!stdout.CanWrite)
      {
        throw new ArgumentException("Stream is not writable.", nameof(stdout));
      }

      if (!stderr.CanWrite)
      {
        throw new ArgumentException("Stream is not writable.", nameof(stderr));
      }

      Enabled = true;
      Stdout = stdout;
      Stderr = stderr;
    }

    /// <inheritdoc />
    public bool Enabled { get; }

    /// <inheritdoc />
    public Stream Stdout { get; }

    /// <inheritdoc />
    public Stream Stderr { get; }

    /// <inheritdoc />
    public void Dispose()
    {
      Stdout.Dispose();
      Stderr.Dispose();
    }
  }
}
