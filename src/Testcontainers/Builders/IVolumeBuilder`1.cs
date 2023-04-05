namespace DotNet.Testcontainers.Builders
{
  using JetBrains.Annotations;

  /// <summary>
  /// A fluent Docker volume builder.
  /// </summary>
  /// <typeparam name="TBuilderEntity">The builder entity.</typeparam>
  [PublicAPI]
  public interface IVolumeBuilder<out TBuilderEntity>
  {
    /// <summary>
    /// Sets the name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithName(string name);
  }
}
