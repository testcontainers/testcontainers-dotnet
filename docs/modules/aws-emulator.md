# AWS Emulator

[AWS Emulator](https://github.com/aws/aws-emulator) is a local emulator of the AWS APIs. It runs a single HTTPS endpoint, generates a self-signed certificate and credentials during the startup, and requires each request to be signed with these credentials.

!!! note

    AWS does not publish an official AWS Emulator image yet. The examples use an [unofficial community image](https://github.com/xSAVIKx/aws-emulator-docker). Any image works as long as it uses the `aws-emulator` executable as its entrypoint.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.AwsEmulator
```

You can start an AWS Emulator container instance from any .NET application. This example uses the default configuration:

=== "Create Container Instance"
    ```csharp
    --8<-- "tests/Testcontainers.AwsEmulator.Tests/AwsEmulatorContainerTest.cs:CreateAwsEmulatorContainer"
    ```

This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

=== "Usage Example"
    ```csharp
    --8<-- "tests/Testcontainers.AwsEmulator.Tests/AwsEmulatorContainerTest.cs:UseAwsEmulatorContainer"
    ```

The test example creates an S3 bucket, uploads an object and downloads it again. Use `GetConnectionString()` as the service URL of the AWS client and `AwsEmulatorBuilder.Region` as the authentication region.

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.AwsEmulator.Tests/Testcontainers.AwsEmulator.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"

## Access AWS Emulator

The emulator generates its credentials and its certificate during the startup. Read them after the container has started:

```csharp title="Gets the AWS Emulator connection string"
string connectionString = _awsEmulatorContainer.GetConnectionString();
```

```csharp title="Gets the AWS Emulator credentials"
AwsEmulatorCredentials credentials = await _awsEmulatorContainer.GetCredentialsAsync();
```

```csharp title="Gets the AWS Emulator certificate"
X509Certificate2 certificate = await _awsEmulatorContainer.GetCertificateAsync();
```

The emulator rejects requests that are not signed with the generated credentials. The credentials are valid for the emulator instance only and cannot authenticate to AWS.

## Trust the certificate

The emulator accepts HTTPS requests only. Instead of accepting any certificate, configure the AWS client to trust the self-signed certificate of the emulator instance. In this example, the HTTP client factory validates the certificate against the certificate of the emulator. The factory is assigned to the `HttpClientFactory` property of the AWS client configuration, as shown in the usage example:

=== "Configure the HTTP Client Factory"
    ```csharp
    --8<-- "tests/Testcontainers.AwsEmulator.Tests/AwsEmulatorHttpClientFactory.cs:ConfigureAwsEmulatorHttpClientFactory"
    ```

!!! note

    The certificate is valid for `localhost`, `127.0.0.1` and `::1`. If the Docker host is reached through another hostname or IP address, the certificate validation fails because of the name mismatch.

## Limitations

Some AWS services return URLs or open additional listeners for their resources. These URLs contain the address and port of the emulator inside the container, and the module does not rewrite them or map the additional ports. For example, the SQS queue URL contains the container port. The AWS client uses it to identify the queue, but sends the requests to the configured service URL.

## Default configuration

--8<-- "src/Testcontainers.AwsEmulator/AwsEmulatorBuilder.Generated.md"