namespace DotNet.Testcontainers.Services
{
  using System.Collections.Generic;
  using System.Linq;

  internal sealed class TestcontainersRegistryService
  {
    private readonly object RegisteredContainersPadLock = new object();

    private readonly IDictionary<string, bool> RegisteredContainers = new Dictionary<string, bool>();

    public IEnumerable<string> GetRegisteredContainers()
    {
      lock (this.RegisteredContainersPadLock)
      {
        return this.RegisteredContainers.Where(registeredContainer => true.Equals(registeredContainer.Value)).Select(registeredContainer => registeredContainer.Key).ToArray();
      }
    }

    public void Register(string id, bool cleanUp = false)
    {
      lock (this.RegisteredContainersPadLock)
      {
        this.RegisteredContainers.Add(id, cleanUp);
      }
    }

    public void Unregister(string id)
    {
      lock (this.RegisteredContainersPadLock)
      {
        this.RegisteredContainers.Remove(id);
      }
    }
  }
}
