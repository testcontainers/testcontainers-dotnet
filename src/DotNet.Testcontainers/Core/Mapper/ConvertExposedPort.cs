namespace DotNet.Testcontainers.Core.Mapper
{
  using System.Collections.Generic;
  using System.Linq;
  using Docker.DotNet.Models;

  internal class ConvertExposedPort : IConverter<IReadOnlyDictionary<string, string>, IDictionary<string, EmptyStruct>>
  {
    public IDictionary<string, EmptyStruct> Convert(IReadOnlyDictionary<string, string> source)
    {
      return source.ToDictionary(exposedPort => $"{exposedPort.Key}/tcp", exposedPort => default(EmptyStruct));
    }
  }
}
