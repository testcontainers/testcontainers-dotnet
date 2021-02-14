namespace DotNet.Testcontainers.Services
{
  using System.Collections.Generic;
  using System.Linq;

  internal sealed class TestcontainersRegistryService
  {
    private readonly IDictionary<string, bool> registeredContainers = new Dictionary<string, bool>();

    public IEnumerable<string> GetRegisteredContainers()
    {
      return this.registeredContainers.Where(registeredContainer => true.Equals(registeredContainer.Value)).Select(registeredContainer => registeredContainer.Key);
    }

    public void Register(string id, bool cleanUp = false)
    {
      this.registeredContainers.Add(id, cleanUp);
    }

    public void Unregister(string id)
    {
      this.registeredContainers.Remove(id);
    }
  }
}
