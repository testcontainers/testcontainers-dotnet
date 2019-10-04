namespace DotNet.Testcontainers.Clients
{
  using System.Collections.Generic;
  using System.Linq;

  internal static class ContainerRegistry
  {
    private static readonly object RegisteredContainersPadLock = new object();

    private static readonly IDictionary<string, bool> RegisteredContainers = new Dictionary<string, bool>();

    public static IEnumerable<string> GetRegisteredContainers()
    {
      lock (RegisteredContainersPadLock)
      {
        return RegisteredContainers.Where(registeredContainer => true.Equals(registeredContainer.Value)).Select(registeredContainer => registeredContainer.Key);
      }
    }

    public static void Register(string id, bool cleanUp = false)
    {
      lock (RegisteredContainersPadLock)
      {
        RegisteredContainers.Add(id, cleanUp);
      }
    }

    public static void Unregister(string id)
    {
      lock (RegisteredContainersPadLock)
      {
        RegisteredContainers.Remove(id);
      }
    }
  }
}
