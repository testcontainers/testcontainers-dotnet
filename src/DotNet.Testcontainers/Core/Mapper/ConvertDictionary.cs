namespace DotNet.Testcontainers.Core.Mapper.Converters
{
  using System.Collections.Generic;
  using System.Linq;

  internal class ConvertDictionary : IConverter<IReadOnlyDictionary<string, string>, IDictionary<string, string>>
  {
    public IDictionary<string, string> Convert(IReadOnlyDictionary<string, string> source)
    {
      return source.ToDictionary(item => item.Key, item => item.Value);
    }
  }
}
