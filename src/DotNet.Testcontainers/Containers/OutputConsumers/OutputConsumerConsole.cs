namespace DotNet.Testcontainers.Containers.OutputConsumers
{
  using System;
  using System.IO;

  public class OutputConsumerConsole : IOutputConsumer, IDisposable
  {
    private readonly TextWriter defaultOut;

    private readonly TextWriter defaultErr;

    private readonly StreamWriter stdout;

    private readonly StreamWriter stderr;

    private bool disposed;

    public OutputConsumerConsole() : this(Console.OpenStandardOutput(), Console.OpenStandardError())
    {
    }

    public OutputConsumerConsole(Stream stdout, Stream stderr)
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

    ~OutputConsumerConsole()
    {
      this.Dispose(false);
    }

    public Stream Stdout => this.stdout.BaseStream;

    public Stream Stderr => this.stderr.BaseStream;

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
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
