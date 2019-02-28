namespace DotNet.Testcontainers.Core.Mapper
{
  using System.Collections.Generic;

  internal abstract class DictionaryConverter<T> : BaseConverter<IReadOnlyDictionary<string, string>, T>
  {
    protected DictionaryConverter() : base(string.Empty)
    {
    }

    protected DictionaryConverter(string name) : base(name)
    {
    }
  }
}
