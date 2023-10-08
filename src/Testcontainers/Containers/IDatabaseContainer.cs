using JetBrains.Annotations;

namespace DotNet.Testcontainers.Containers
{
  /// <summary>
  /// Represents a database container instance.
  /// </summary>
  [PublicAPI]
  public interface IDatabaseContainer
  {
    /// <summary>
    /// Gets the database connection string.
    /// </summary>
    /// <returns>The database connection string.</returns>
    [NotNull]
    string GetConnectionString();
  }
}
