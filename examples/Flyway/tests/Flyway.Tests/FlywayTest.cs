namespace Flyway.Tests;

public sealed class FlywayTest : IClassFixture<DbFixture>, IDisposable
{
    private readonly DbConnection _dbConnection;

    public FlywayTest(DbFixture db)
    {
        _dbConnection = db.DbConnection;
        _dbConnection.Open();
    }

    public void Dispose()
    {
        _dbConnection.Dispose();
    }

    [Fact]
    public void UsersTableContainsJohnDoe()
    {
        // Given
        using var command = _dbConnection.CreateCommand();
        command.CommandText = "SELECT username, email, age FROM users;";

        // When
        using var dataReader = command.ExecuteReader();

        // Then
        Assert.True(dataReader.Read());
        Assert.Equal("john_doe", dataReader.GetString(0));
        Assert.Equal("john@example.com", dataReader.GetString(1));
        Assert.Equal(30, dataReader.GetInt32(2));
    }
}