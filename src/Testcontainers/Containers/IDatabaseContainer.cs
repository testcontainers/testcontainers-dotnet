namespace DotNet.Testcontainers.Containers
{
  using JetBrains.Annotations;

  /// <summary>
  /// Represents a database container instance that can be accessed with an ADO.NET provider.
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
