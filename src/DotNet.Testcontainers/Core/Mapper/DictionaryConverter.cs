namespace DotNet.Testcontainers.Core.Mapper
{
  using System.Collections.Generic;

  /// <summary>
  /// Converts or maps Testcontainers dictionary configurations to official Docker configurations.
  /// </summary>
  /// <typeparam name="T">Official Docker configuration type.</typeparam>
  internal abstract class DictionaryConverter<T> : BaseConverter<IReadOnlyDictionary<string, string>, T>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DictionaryConverter{T}"/> class without converter name.
    /// </summary>
    protected DictionaryConverter() : this(string.Empty)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DictionaryConverter{T}"/> class.
    /// </summary>
    /// <param name="name">Name of the converter.</param>
    protected DictionaryConverter(string name) : base(name)
    {
    }
  }
}
