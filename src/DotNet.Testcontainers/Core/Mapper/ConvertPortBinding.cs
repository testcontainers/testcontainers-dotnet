namespace DotNet.Testcontainers.Core.Mapper.Converters
{
  using System.Collections.Generic;
  using System.Linq;
  using Docker.DotNet.Models;

  internal class ConvertPortBinding : IConverter<IReadOnlyDictionary<string, string>, IDictionary<string, IList<PortBinding>>>
  {
    public IDictionary<string, IList<PortBinding>> Convert(IReadOnlyDictionary<string, string> source)
    {
      return source.ToDictionary(binding => $"{binding.Value}/tcp", binding => new List<PortBinding> { new PortBinding { HostPort = binding.Key } } as IList<PortBinding>);
    }
  }
}
