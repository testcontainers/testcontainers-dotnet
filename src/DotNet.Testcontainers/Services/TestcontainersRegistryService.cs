namespace DotNet.Testcontainers.Services
{
  using System.Collections.Generic;
  using System.Linq;

  internal sealed class TestcontainersRegistryService
  {
    private readonly object registeredContainersPadLock = new object();

    private readonly IDictionary<string, bool> registeredContainers = new Dictionary<string, bool>();

    public IEnumerable<string> GetRegisteredContainers()
    {
      lock (this.registeredContainersPadLock)
      {
        return this.registeredContainers.Where(registeredContainer => true.Equals(registeredContainer.Value)).Select(registeredContainer => registeredContainer.Key).ToArray();
      }
    }

    public void Register(string id, bool cleanUp = false)
    {
      lock (this.registeredContainersPadLock)
      {
        this.registeredContainers.Add(id, cleanUp);
      }
    }

    public void Unregister(string id)
    {
      lock (this.registeredContainersPadLock)
      {
        this.registeredContainers.Remove(id);
      }
    }
  }
}
