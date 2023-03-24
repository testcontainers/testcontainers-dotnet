namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.IO;

  /// <inheritdoc cref="IOutputConsumer" />
  internal sealed class RedirectStdoutAndStderrToStream : IOutputConsumer
  {
    private readonly StreamWriter _stdout;

    private readonly StreamWriter _stderr;

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
    public RedirectStdoutAndStderrToStream(Stream stdout, Stream stderr)
    {
      Enabled = stdout.CanWrite || stderr.CanWrite;
      _stdout = new StreamWriter(stdout);
      _stdout.AutoFlush = true;
      _stderr = new StreamWriter(stderr);
      _stderr.AutoFlush = true;
    }

    /// <inheritdoc />
    public bool Enabled { get; }

    /// <inheritdoc />
    public Stream Stdout
      => _stdout.BaseStream;

    /// <inheritdoc />
    public Stream Stderr
      => _stderr.BaseStream;

    /// <inheritdoc />
    public void Dispose()
    {
      _stdout.Dispose();
      _stderr.Dispose();
    }
  }
}
