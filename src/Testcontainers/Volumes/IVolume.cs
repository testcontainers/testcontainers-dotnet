namespace DotNet.Testcontainers.Volumes
{
  using System;
  using JetBrains.Annotations;

  /// <summary>
  /// A volume instance.
  /// </summary>
  [PublicAPI]
  public interface IVolume : IFutureResource
  {
    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <exception cref="InvalidOperationException">Volume has not been created.</exception>
    [NotNull]
    string Name { get; }
  }
}
