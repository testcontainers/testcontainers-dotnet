namespace DotNet.Testcontainers.Containers.OutputConsumers.Common
{
  using System;
  using System.IO;

  /// <inheritdoc cref="IOutputConsumer" />
  internal sealed class RedirectStdoutAndStderrToStream : IOutputConsumer
  {
    private readonly TextWriter defaultOut;

    private readonly TextWriter defaultErr;

    private readonly StreamWriter stdout;

    private readonly StreamWriter stderr;

    private bool disposed;

    public RedirectStdoutAndStderrToStream() : this(
      Console.OpenStandardOutput(), Console.OpenStandardError())
    {
    }

    public RedirectStdoutAndStderrToStream(Stream stdout, Stream stderr)
    {
      this.defaultOut = Console.Out;
      this.defaultErr = Console.Error;

      this.stdout = new StreamWriter(stdout)
      {
        AutoFlush = true,
      };

      this.stderr = new StreamWriter(stderr)
      {
        AutoFlush = true,
      };

      Console.SetOut(this.stdout);
      Console.SetError(this.stderr);
    }

    ~RedirectStdoutAndStderrToStream()
    {
      this.Dispose(false);
    }

    /// <inheritdoc />
    public Stream Stdout => this.stdout.BaseStream;

    /// <inheritdoc />
    public Stream Stderr => this.stderr.BaseStream;

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
      if (this.disposed)
      {
        return;
      }

      Console.SetOut(this.defaultOut);
      Console.SetError(this.defaultErr);

      this.stdout.Dispose();
      this.stderr.Dispose();

      this.disposed = true;
    }
  }
}
