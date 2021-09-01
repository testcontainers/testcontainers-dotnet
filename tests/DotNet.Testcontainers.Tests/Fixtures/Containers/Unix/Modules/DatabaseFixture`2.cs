namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;
  using Xunit;

  /// <summary>
  /// A Testcontainers database class fixture.
  /// </summary>
  /// <typeparam name="TDockerContainer">Type of <see cref="ITestcontainersContainer" />.</typeparam>
  /// <typeparam name="TDatabaseConnection">Type of database connection.</typeparam>
#pragma warning disable SA1649

  public abstract class DatabaseFixture<TDockerContainer, TDatabaseConnection> : IAsyncLifetime, IDisposable
    where TDockerContainer : ITestcontainersContainer

#pragma warning restore SA1649
  {
    /// <summary>
    /// Gets or sets the Testcontainers.
    /// </summary>
    public TDockerContainer Container { get; protected set; }

    /// <summary>
    /// Gets or sets the database connection.
    /// </summary>
    public TDatabaseConnection Connection { get; protected set; }

    /// <inheritdoc />
    public abstract Task InitializeAsync();

    /// <inheritdoc />
    public abstract Task DisposeAsync();

    /// <inheritdoc />
    public abstract void Dispose();
  }
}
