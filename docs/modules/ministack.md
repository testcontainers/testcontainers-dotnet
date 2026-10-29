# MiniStack

[MiniStack](https://github.com/ministackorg/ministack) is a free, open-source AWS emulator that serves its AWS services on a single port.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.MiniStack
```

You can start a MiniStack container instance from any .NET application. Here, we create different container instances and pass them to the base test class. This allows us to test different configurations.

=== "Create Container Instance"
    ```csharp
    --8<-- "tests/Testcontainers.MiniStack.Tests/MiniStackContainerTest.cs:CreateMiniStackContainer"
    ```

This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

=== "Usage Example"
    ```csharp
    --8<-- "tests/Testcontainers.MiniStack.Tests/MiniStackContainerTest.cs:UseMiniStackContainer"
    ```

The test example creates an S3 bucket, writes an object and reads it back. Use `GetConnectionString()` as the AWS SDK `ServiceURL`, and `GetRegion()`, `GetAccessKey()` and `GetSecretKey()` for the region and credentials. By default, MiniStack uses the region `us-east-1` and the credentials `test`/`test`.

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.MiniStack.Tests/Testcontainers.MiniStack.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"

## Configuration

The builder sets MiniStack's environment variables for the most common options:

=== "Custom Configuration"
    ```csharp
    --8<-- "tests/Testcontainers.MiniStack.Tests/MiniStackContainerTest.cs:CustomMiniStackContainer"
    ```

- `WithRegion(string)` sets `MINISTACK_REGION`, the region MiniStack uses in the ARNs it generates.
- `WithCredentials(string, string)` sets `AWS_ACCESS_KEY_ID` and `AWS_SECRET_ACCESS_KEY`. MiniStack treats a 12-digit access key as the account ID.
- `WithPersistence()` sets `PERSIST_STATE=1` and `S3_PERSIST=1`, so service state and S3 objects are written to disk and survive a restart of the same container.
- `WithImdsV2Required()` sets `MINISTACK_IMDS_V2_REQUIRED=1`, so instance metadata requests without a session token are rejected.
- `WithServices(params string[])` sets `SERVICES`, so MiniStack loads only the listed services, e.g. `WithServices("s3", "sqs")`.

`GetMiniStackVersion()` returns the image tag, or `null` for an image referenced only by digest. `GetMiniStackVersionAsync()` returns the version the running server reports, which also resolves tags such as `latest`.

If Testcontainers is configured with a hub image name prefix, the module passes it to MiniStack as `MINISTACK_IMAGE_PREFIX`, so the images MiniStack pulls for real infrastructure come from the same registry.

## Init and ready scripts

MiniStack runs shell (`.sh`) and Python (`.py`) scripts in alphabetical order of their file names. Use `WithInitScript(string)` or `WithInitScriptContent(string, string)` for boot scripts, which run before MiniStack serves requests. Use `WithReadyScript(string)` or `WithReadyScriptContent(string, string)` for ready scripts, which run once MiniStack serves requests and can create AWS resources:

=== "Script Configuration"
    ```csharp
    --8<-- "tests/Testcontainers.MiniStack.Tests/MiniStackScriptTest.cs:ScriptMiniStackContainer"
    ```

The ready scripts can use the AWS CLI (`aws`, `awslocal`) and boto3, which MiniStack preconfigures with its endpoint, region and credentials. The container is ready once `/_ministack/ready` returns HTTP 200, which happens after all ready scripts have run. If a ready script fails, `StartAsync` throws an `InvalidOperationException`, and the script output is in the container logs.

Older MiniStack images have no `/_ministack/ready` endpoint and return HTTP 404. For those, the container is ready once `/_ministack/health` returns HTTP 200, the module does not wait for ready scripts, and it cannot detect a failed one.

## Real infrastructure

`WithRealInfrastructure()` bind-mounts the Docker socket into the container. It uses the same socket path as the Resource Reaper, including the `TESTCONTAINERS_DOCKER_SOCKET_OVERRIDE` setting. MiniStack then runs real containers for services such as RDS, ElastiCache, ECS and Lambda:

=== "Real Infrastructure Configuration"
    ```csharp
    --8<-- "tests/Testcontainers.MiniStack.Tests/MiniStackRealInfrastructureTest.cs:RealInfrastructureMiniStackContainer"
    ```

By default, RDS reports the database container's address on the Docker network, which a test running on a macOS or Windows host cannot reach. `WithRdsPublicEndpoint()` sets `MINISTACK_RDS_PUBLIC_ENDPOINT=1`, so RDS reports `localhost` and the published host port instead. That address works when the tests and the Docker daemon run on the same machine.

!!! warning

    Anything running inside MiniStack, including the code of a Lambda function, gets full control over the host's container engine. Use real infrastructure only on trusted machines and CI runners.

The Resource Reaper does not know about the containers MiniStack starts. When MiniStack is stopped, it removes them itself. Disposing the container kills MiniStack before it can do that, so the module then removes the containers and volumes that carry its `ministack.instance` label. MiniStack builds that label from `MINISTACK_HOSTNAME` (or the container hostname) and the gateway port. Containers of another MiniStack instance on the same Docker daemon are left alone, unless both instances set the same `MINISTACK_HOSTNAME` and port.
