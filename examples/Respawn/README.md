# Testcontainers for .NET Respawn example

This example demonstrates how to use Respawn to reset the state of a PostgreSQL database between tests, ensuring isolation and consistency. The `RespawnTest` test class interacts with a pre-configured database, using Respawn to clear the data before each test run. This allows the tests to execute in a clean environment without interfering with each other.

```console
git clone --branch develop git@github.com:testcontainers/testcontainers-dotnet.git
cd ./testcontainers-dotnet/examples/Respawn/
dotnet test Respawn.sln --configuration=Release
```