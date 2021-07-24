namespace DotNet.Testcontainers.Clients
{
  using System.Collections.Generic;

  /// <summary>
  /// Converts or maps Testcontainers dictionary configurations to official Docker configurations.
  /// </summary>
  /// <typeparam name="TTarget">Official Docker configuration type.</typeparam>
  internal abstract class DictionaryConverter<TTarget> : BaseConverter<IEnumerable<KeyValuePair<string, string>>, TTarget>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DictionaryConverter{TTarget}" /> class.
    /// </summary>
    /// <param name="name">Name of the converter.</param>
    protected DictionaryConverter(string name)
      : base(name)
    {
    }
  }
}
