namespace DotNet.Testcontainers.Builders
{
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;

  /// <summary>
  /// A fluent Docker network builder.
  /// </summary>
  /// <typeparam name="TBuilderEntity">The builder entity.</typeparam>
  [PublicAPI]
  public interface INetworkBuilder<out TBuilderEntity>
  {
    /// <summary>
    /// Sets the name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithName(string name);

    /// <summary>
    /// Sets the driver.
    /// </summary>
    /// <param name="driver">The driver.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithDriver(NetworkDriver driver);

    /// <summary>
    /// Sets the network option.
    /// </summary>
    /// <param name="name">The network option name.</param>
    /// <param name="value">The network option value.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithOption(string name, string value);
  }
}
