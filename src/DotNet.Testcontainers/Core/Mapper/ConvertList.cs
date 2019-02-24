namespace DotNet.Testcontainers.Core.Mapper.Converters
{
  using System.Collections.Generic;
  using System.Linq;

  internal class ConvertList : IConverter<IReadOnlyCollection<string>, IList<string>>
  {
    public IList<string> Convert(IReadOnlyCollection<string> source)
    {
      return source.ToList();
    }
  }
}
