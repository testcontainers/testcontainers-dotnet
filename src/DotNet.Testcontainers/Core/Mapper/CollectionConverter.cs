namespace DotNet.Testcontainers.Core.Mapper
{
  using System.Collections.Generic;

  internal abstract class CollectionConverter<T> : BaseConverter<IReadOnlyCollection<string>, T>
  {
    protected CollectionConverter() : base(string.Empty)
    {
    }

    protected CollectionConverter(string name) : base(name)
    {
    }
  }
}
