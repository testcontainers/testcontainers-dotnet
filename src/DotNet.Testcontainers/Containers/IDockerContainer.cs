namespace DotNet.Testcontainers.Containers
{
  using System;

  public interface IDockerContainer : IDisposable
  {
    void Pull();

    void Run();

    void Start();

    void Stop();
  }
}
