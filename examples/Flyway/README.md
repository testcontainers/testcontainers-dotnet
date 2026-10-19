# Testcontainers for .NET Flyway example

This example demonstrates how to use Testcontainers in conjunction with Flyway to execute database migrations and prepare a dependent database before running tests. The `FlywayTest` test class executes its tests against the pre-configured PostgreSQL database, interacting with the table that is created, altered, and seeded beforehand. This test class receives a class fixture, which provides access to the prepared database through the `DbConnection` property. The database is started, created, and seeded once and is shared across the tests within the `FlywayTest` test collection. Checkout and run the tests on your machine:

```console
git clone --branch develop git@github.com:testcontainers/testcontainers-dotnet.git
cd ./testcontainers-dotnet/examples/Flyway/
dotnet test Flyway.sln --configuration=Release
```
