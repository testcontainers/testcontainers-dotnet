namespace DotNet.Testcontainers.Containers
{
  using System;

  public interface IDockerContainer : IDisposable
  {
    void Start();

    void Stop();
  }
}
