# Docker Compose

`ComposeBuilder` and `ComposeContainer` run a Docker Compose project as part of your tests. Docker Compose runs inside a container (`docker compose up` / `docker compose down`), you do not need Docker Compose installed on the test host. Testcontainers copies the Docker Compose files, and the directories they live in, into that container, and mounts the Docker socket so Docker Compose can reach the Docker daemon.

```csharp
--8<-- "tests/Testcontainers.Platform.Linux.Tests/ComposeContainerExampleTest.cs:CreateComposeContainer"
```

## Docker Compose files

`WithComposeFile(params string[])` accepts one or more Docker Compose file paths. Each file keeps the path it has on the test host, which lets Docker Compose resolve relative references, such as bind mount sources or included Docker Compose files, the same way it would outside a container.

By default, the entire directory of each Docker Compose file is copied into the container. If that directory also contains files you do not want copied, such as build output, restrict the copy with `WithCopyFilesInContainer(params string[])`. It copies the Docker Compose files and the listed files and directories only, resolved relative to the directory of the first Docker Compose file.

## Waiting for services

Use `WaitingFor(string, IWaitForContainerOS)` to wait for a service without exposing it, and `WithExposedService(string, ushort, IWaitForContainerOS)` to expose a service port and wait for it at the same time. `WithExposedService(string, ushort)` exposes a port without an additional wait strategy. The readiness check then only waits until the port accepts connections.

An exposed service port does not need to be published in the Docker Compose file. An ambassador container proxies the port and makes it accessible to the test host, so wait strategies work against Docker Compose services the same way they work against any other container. Resolve the address with `GetServiceHost(string, ushort)` and `GetServicePort(string, ushort)`.

```csharp
--8<-- "tests/Testcontainers.Platform.Linux.Tests/ComposeContainerExampleTest.cs:ConnectToExposedService"
```

!!! note

    `docker compose up` already starts the services. The wait strategies of a Docker Compose service run against the already running container. They do not start it.

## Getting the service container

`GetServiceContainer(string)` returns the `IContainer` that belongs to a Docker Compose service, for example to read its logs or run a command inside it. Docker Compose owns its lifecycle. Stopping or disposing the returned container has no effect. Stop or dispose the `ComposeContainer` instead.

```csharp
--8<-- "tests/Testcontainers.Platform.Linux.Tests/ComposeContainerExampleTest.cs:GetServiceContainer"
```

## Scaled services

`WithScaledService(string, ushort)` runs more than one container for a Docker Compose service. Each container is one instance, addressed by the service name and an instance number starting at `1`. Every `*Service` member has an `*ServiceInstance` counterpart that takes the instance number, for example `WithExposedServiceInstance(string, ushort, ushort)` and `GetServiceInstanceContainer(string, ushort)`. Members without an instance number always address the first instance.

```csharp
var composeContainer = new ComposeBuilder(image)
  .WithComposeFile(composeFilePath)
  .WithScaledService("web", 2)
  .WithExposedServiceInstance("web", 1, 80)
  .WithExposedServiceInstance("web", 2, 80)
  .Build();
```

## Project name

Testcontainers generates a random Docker Compose project name for every `ComposeContainer`, so concurrent test runs do not collide. Use `WithProjectNamePrefix(string)` to make the generated name easier to recognize, for example in Docker Desktop or `docker compose ls`. Testcontainers appends a random suffix to the prefix, and reads it back through `ComposeContainer.ProjectName`.

```csharp
_ = new ComposeBuilder(image)
  .WithComposeFile(composeFilePath)
  .WithProjectNamePrefix("checkout-service");
```

## Pulling images

Testcontainers pulls the Docker Compose service images from the test host before `docker compose up` runs, so the Docker credentials and credential helpers of the test host apply. Docker Compose running inside its own container does not have access to them. This is enabled by default. A failing pull does not fail the start. Docker Compose still tries to pull the image itself. Disable it with `WithPull(false)` if your images are already present on the Docker host.

!!! note

    `ComposeContainer` does not support `WithReuse(true)`.
