namespace DotNet.Testcontainers.Core.Mapper.Converters
{
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using Docker.DotNet.Models;

  internal class ConvertMount : IConverter<IReadOnlyDictionary<string, string>, IList<Mount>>
  {
    public IList<Mount> Convert(IReadOnlyDictionary<string, string> source)
    {
      return source.Select(mount => new Mount { Source = Path.GetFullPath(mount.Key), Target = mount.Value, Type = "bind" }).ToList();
    }
  }
}
