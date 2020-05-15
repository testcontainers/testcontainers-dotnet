namespace DotNet.Testcontainers.Clients
{
  using System.Collections.Generic;

  internal sealed class FilterByProperty : Dictionary<string, IDictionary<string, bool>>
  {
    public FilterByProperty(string property, string value)
    {
      this.Add(property, new Dictionary<string, bool> { { value, true } });
    }
  }
}
