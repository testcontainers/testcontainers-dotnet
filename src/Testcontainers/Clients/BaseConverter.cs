namespace DotNet.Testcontainers.Clients
{
  using JetBrains.Annotations;

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
      _ = name;
    }

    /// <summary>
    /// Converts or maps the Testcontainers configuration to the official Docker configuration.
    /// </summary>
    /// <param name="source">Testcontainers configuration.</param>
    /// <returns>Official Docker configuration.</returns>
    [CanBeNull]
    public abstract TTarget Convert(TSource source);
  }
}
