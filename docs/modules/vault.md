# HashiCorp Vault

[HashiCorp Vault](https://developer.hashicorp.com/vault) is an identity-based secrets and encryption management system.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.Vault
```

You can start a Vault container instance from any .NET application. This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

=== "Usage Example"
    ```csharp
    --8<-- "tests/Testcontainers.Vault.Tests/VaultContainerTest.cs:UseVaultContainer"
    ```

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.Vault.Tests/Testcontainers.Vault.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

## Root token

The container runs Vault in dev mode. The root token is `root` unless you set a different one with `WithRootToken(string)`. `GetRootToken()` returns it, and the Vault CLI inside the container uses the same token.

## Init commands

`WithInitCommand(params string[])` adds Vault CLI commands that run in order after the container has started. Pass the commands without the leading `vault`, for example to enable a secrets engine or to write a secret:

```csharp
--8<-- "tests/Testcontainers.Vault.Tests/VaultInitCommandTest.cs:UseVaultInitCommands"
```

If a command fails, the container start fails with an `ExecFailedException` that contains the exit code and the output of the command.

--8<-- "docs/modules/_call_out_test_projects.txt"
