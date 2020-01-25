namespace DotNet.Testcontainers.Internals.Mappers
{
  /// <summary>
  /// Converts or maps the Testcontainers configuration to the official Docker configuration.
  /// </summary>
  /// <typeparam name="TSource">Testcontainers configuration type.</typeparam>
  /// <typeparam name="TTarget">Official Docker configuration type.</typeparam>
  internal abstract class BaseConverter<TSource, TTarget>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseConverter{TSource, TTarget}" /> class.
    /// </summary>
    /// <param name="name">Name of the converter.</param>
    protected BaseConverter(string name)
    {
      this.Name = name;
    }

    /// <summary>Gets the name of the converter.</summary>
    /// <value>Identifies the converter, if source and target types are frequently used.</value>
    public string Name { get; }

    /// <summary>
    /// Converts or maps the Testcontainers configuration to the official Docker configuration.
    /// </summary>
    /// <param name="source">Testcontainers configuration.</param>
    /// <returns>Official Docker configuration.</returns>
    public abstract TTarget Convert(TSource source);
  }
}
