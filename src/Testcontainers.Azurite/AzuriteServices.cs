namespace Testcontainers.Azurite;

/// <summary>
///   Azurite services.
/// </summary>
[PublicAPI]
public enum AzuriteServices
{
  /// <summary>
  ///   The blob service.
  /// </summary>
  Blob = 1,

  /// <summary>
  ///   The queue service.
  /// </summary>
  Queue = 2,

  /// <summary>
  ///   The table service.
  /// </summary>
  Table = 4,

  /// <summary>
  ///   All services.
  /// </summary>
  All = 7,
}