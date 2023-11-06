namespace DotNet.Testcontainers.Clients
{
  using System.Collections.Concurrent;
  using System.Collections.Generic;

  internal sealed class FilterByProperty : ConcurrentDictionary<string, IDictionary<string, bool>>
  {
    public FilterByProperty Add(string property, string value)
    {
      var values = GetOrAdd(property, _ => new Dictionary<string, bool>());
      values[value] = true;
      return this;
    }
  }
}
