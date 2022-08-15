namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.IO;

  /// <inheritdoc cref="IOutputConsumer" />
  internal sealed class RedirectStdoutAndStderrToStream : IOutputConsumer
  {
    private readonly StreamWriter stdout;

    private readonly StreamWriter stderr;

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
      this.Enabled = stdout.CanWrite || stderr.CanWrite;
      this.stdout = new StreamWriter(stdout);
      this.stdout.AutoFlush = true;
      this.stderr = new StreamWriter(stderr);
      this.stderr.AutoFlush = true;
    }

    /// <inheritdoc />
    public bool Enabled { get; }

    /// <inheritdoc />
    public Stream Stdout
      => this.stdout.BaseStream;

    /// <inheritdoc />
    public Stream Stderr
      => this.stderr.BaseStream;

    /// <inheritdoc />
    public void Dispose()
    {
      this.stdout.Dispose();
      this.stderr.Dispose();
    }
  }
}
