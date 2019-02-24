namespace DotNet.Testcontainers.Core.Containers
{
  using System;

  public interface IDockerContainer : IDisposable
  {
    string Id { get; }

    string Name { get; }

    void Start();

    void Stop();
  }
}
