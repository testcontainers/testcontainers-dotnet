namespace DotNet.Testcontainers.Core.Mapper.Converters
{
  using System.Collections.Generic;
  using System.Linq;

  internal class ConvertList : IConverter<IReadOnlyCollection<string>, IList<string>>,
    IConverter<IReadOnlyDictionary<string, string>, IList<string>>
  {
    public IList<string> Convert(IReadOnlyCollection<string> source)
    {
      return source.ToList();
    }

    public IList<string> Convert(IReadOnlyDictionary<string, string> source)
    {
      return source.Select(item => $"{item.Key}={item.Value}").ToList();
    }
  }
}
