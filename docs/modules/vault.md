# HashiCorp Vault

[HashiCorp Vault](https://developer.hashicorp.com/vault) is an identity-based secrets and encryption management system.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.Vault
```

You can start a Vault container instance from any .NET application. Here, we create different container instances and pass them to the base test class. This allows us to test different configurations.

=== "Create Container Instance"
    ```csharp
    --8<-- "tests/Testcontainers.Vault.Tests/VaultContainerTest.cs:CreateVaultContainer"
    ```

This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

=== "Usage Example"
    ```csharp
    --8<-- "tests/Testcontainers.Vault.Tests/VaultContainerTest.cs:UseVaultContainer"
    ```

The test example writes a secret to the key/value secrets engine and reads it back. Use `GetBaseAddress()` to configure the Vault client. The container runs Vault in dev mode, and the default configuration uses the root token `root`. Use `WithRootToken(string)` to configure a different root token.

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.Vault.Tests/Testcontainers.Vault.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"

## Init commands

Use `WithInitCommand(params string[])` to run Vault CLI commands after the container has started, for example to enable a secrets engine or to write a secret. The commands run in order, without the leading `vault`, and use the configured root token:

=== "Init Command Configuration"
    ```csharp
    --8<-- "tests/Testcontainers.Vault.Tests/VaultContainerTest.cs:InitCommandContainerConfiguration"
    ```

If a command fails, the container start throws an `ExecFailedException` that contains the exit code and the output of the command. In dev mode, Vault keeps its data in memory. A restarted container starts without any data, so the init commands run on every start.
