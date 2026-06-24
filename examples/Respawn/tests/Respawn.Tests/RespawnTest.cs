namespace Respawn.Tests;

public sealed class RespawnTest : IClassFixture<DbFixture>, IDisposable
{
    private readonly DbConnection _dbConnection;

    public RespawnTest(DbFixture db)
    {
        _dbConnection = db.DbConnection;
        _dbConnection.Open();
    }

    public void Dispose()
    {
        _dbConnection.Dispose();
    }

    [Theory]
    [InlineData("jane_doe", "jane@example.com", 30)]
    [InlineData("john_doe", "john@example.com", 30)]
    public async Task UsersTableContainsOneUser(string username, string email, int age)
    {
        // Respawn resets the database and cleans its state. This allows tests to run without
        // interfering with each other. Instead of deleting data at the end of a test, rolling
        // back a transaction, or creating a new container instance, Respawn resets the database
        // to a clean, empty state by intelligently deleting data from tables.
        var respawnerOptions = new RespawnerOptions { DbAdapter = DbAdapter.Postgres };
        var respawner = await Respawner.CreateAsync(_dbConnection, respawnerOptions);
        await respawner.ResetAsync(_dbConnection);

        // This test runs twice and inserts a record into the `users` table for each run.  
        // The test counts the number of users and expects it to always be 1. If the database  
        // state is not clean, the assertion fails.
        using var insertCommand = _dbConnection.CreateCommand();

        var dbParamUsername = insertCommand.CreateParameter();
        dbParamUsername.ParameterName = "@username";
        dbParamUsername.Value = username;

        var dbParamEmail = insertCommand.CreateParameter();
        dbParamEmail.ParameterName = "@email";
        dbParamEmail.Value = email;

        var dbParamAge = insertCommand.CreateParameter();
        dbParamAge.ParameterName = "@age";
        dbParamAge.Value = age;

        insertCommand.CommandText = "INSERT INTO users (username, email, age) VALUES (@username, @email, @age)";
        insertCommand.Parameters.Add(dbParamUsername);
        insertCommand.Parameters.Add(dbParamEmail);
        insertCommand.Parameters.Add(dbParamAge);
        insertCommand.ExecuteNonQuery();

        using var selectCommand = _dbConnection.CreateCommand();
        selectCommand.CommandText = "SELECT COUNT(*) FROM users";
        var userCount = Convert.ToInt32(selectCommand.ExecuteScalar());

        Assert.Equal(1, userCount);
    }
}