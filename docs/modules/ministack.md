# MiniStack

[MiniStack](https://github.com/ministackorg/ministack) is a free, open-source AWS emulator. It emulates 60+ AWS services on a single port, is MIT licensed, and requires no account, API key, or sign-up. It is a drop-in replacement for LocalStack and works with any AWS SDK, Terraform, CDK, and Pulumi.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.MiniStack
```

You can start a MiniStack container instance from any .NET application. This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

=== "Usage Example"
    ```csharp
    --8<-- "tests/Testcontainers.MiniStack.Tests/MiniStackContainerTest.cs:UseMiniStackContainer"
    ```

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.MiniStack.Tests/Testcontainers.MiniStack.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"
