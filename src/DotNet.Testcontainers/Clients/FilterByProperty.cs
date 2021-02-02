namespace DotNet.Testcontainers.Clients
{
  using System.Collections.Concurrent;
  using System.Collections.Generic;

  internal sealed class FilterByProperty : ConcurrentDictionary<string, IDictionary<string, bool>>
  {
    public FilterByProperty()
    {
    }

    public FilterByProperty(string property, string value)
    {
      this.Add(property, value);
    }

    public void Add(string property, string value)
    {
      var values = this.GetOrAdd(property, key => new Dictionary<string, bool>());
      values[value] = true;
    }
  }
}
