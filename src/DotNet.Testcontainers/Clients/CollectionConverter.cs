namespace DotNet.Testcontainers.Clients
{
  using System.Collections.Generic;

  /// <summary>
  /// Converts or maps Testcontainers collection configurations to official Docker configurations.
  /// </summary>
  /// <typeparam name="TSource">Testcontainer configuration type.</typeparam>
  /// <typeparam name="TTarget">Official Docker configuration type.</typeparam>
  internal abstract class CollectionConverter<TSource, TTarget> : BaseConverter<IEnumerable<TSource>, IEnumerable<TTarget>>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionConverter{TSource,TTarget}" /> class.
    /// </summary>
    /// <param name="name">Name of the converter.</param>
    protected CollectionConverter(string name)
      : base(name)
    {
    }
  }
}
