namespace DotNet.Testcontainers.Diagnostics
{
  using System.IO;

  public interface IOutputConsumer
  {
    Stream Stdout { get; }

    Stream Stderr { get; }
  }
}
