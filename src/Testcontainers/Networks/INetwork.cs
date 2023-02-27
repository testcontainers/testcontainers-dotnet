namespace DotNet.Testcontainers.Networks
{
  using System;
  using JetBrains.Annotations;

  /// <summary>
  /// A network instance.
  /// </summary>
  [PublicAPI]
  public interface INetwork : IFutureResource
  {
    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <exception cref="InvalidOperationException">Network has not been created.</exception>
    [NotNull]
    string Name { get; }
  }
}
