namespace DotNet.Testcontainers.Core.Mapper
{
  using System.Collections.Generic;

  /// <summary>
  /// Converts or maps Testcontainers collection configurations to official Docker configurations.
  /// </summary>
  /// <typeparam name="T">Official Docker configuration type.</typeparam>
  internal abstract class CollectionConverter<T> : BaseConverter<IReadOnlyCollection<string>, T>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionConverter{T}"/> class without converter name.
    /// </summary>
    protected CollectionConverter() : base(string.Empty)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionConverter{T}"/> class.
    /// </summary>
    /// <param name="name">Name of the converter.</param>
    protected CollectionConverter(string name) : base(name)
    {
    }
  }
}
