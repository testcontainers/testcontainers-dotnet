namespace DotNet.Testcontainers.Containers
{
  using System;

  public interface IDockerContainer : IDisposable
  {
    string Name { get; }

    void Start();

    void Stop();
  }
}
