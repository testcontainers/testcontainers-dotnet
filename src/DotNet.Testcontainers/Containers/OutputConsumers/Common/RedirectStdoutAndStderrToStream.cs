namespace DotNet.Testcontainers.Containers.OutputConsumers.Common
{
  using System;
  using System.IO;

  /// <inheritdoc cref="IOutputConsumer" />
  internal sealed class RedirectStdoutAndStderrToStream : IOutputConsumer
  {
    private readonly StreamWriter stdout;

    private readonly StreamWriter stderr;

    public RedirectStdoutAndStderrToStream() : this(
      Console.OpenStandardOutput(), Console.OpenStandardError())
    {
    }

    public RedirectStdoutAndStderrToStream(Stream stdout, Stream stderr)
    {
      this.stdout = new StreamWriter(stdout) { AutoFlush = true, };
      this.stderr = new StreamWriter(stderr) { AutoFlush = true, };
    }

    /// <inheritdoc />
    public Stream Stdout => this.stdout.BaseStream;

    /// <inheritdoc />
    public Stream Stderr => this.stderr.BaseStream;

    public void Dispose()
    {
      this.stdout.Dispose();
      this.stderr.Dispose();
    }
  }
}
