namespace DotNet.Testcontainers.Containers
{
  using JetBrains.Annotations;

  /// <summary>
  /// A database container instance.
  /// </summary>
  [PublicAPI]
  public interface IDatabaseContainer
  {
    /// <summary>
    /// Gets the connection string for connecting to the database.
    /// </summary>
    /// <returns>The connection string for connecting to the database.</returns>
    string GetConnectionString();
  }
}
