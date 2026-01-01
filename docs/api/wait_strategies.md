# Wait Strategies

Wait strategies are useful to detect if a container is ready for testing (i.e., if the application inside the container is in a usable state). They check different indicators of readiness of the container and complete as soon as they are fulfilled. Per default, Testcontainers will wait until the container is running. For the most images, that is not enough. You can chain different pre-configured wait strategies together or implement your own by implementing the `IWaitUntil` interface.

```csharp
_ = Wait.ForUnixContainer()
  .UntilInternalTcpPortIsAvailable(80)
  .UntilFileExists("/tmp/foo")
  .UntilFileExists("/tmp/bar")
  .AddCustomWaitStrategy(new MyCustomWaitStrategy());
```

In some cases, it might be necessary to configure the behavior of a wait strategy further, being able to cancel the readiness check. The API provides a callback that allows setting additional configurations such as `Retries`, `Interval`, and `Timeout`.

```csharp title="Cancel the readiness check after one minute"
_ = Wait.ForUnixContainer()
  .UntilMessageIsLogged("Server started", o => o.WithTimeout(TimeSpan.FromMinutes(1)));
```

Besides configuring the wait strategy, cancelling a container start can always be done utilizing a [CancellationToken](create_docker_container.md#canceling-a-container-start).

## Wait strategy modes

Wait strategy modes define how Testcontainers for .NET handles container readiness checks. By default, wait strategies assume the container remains running throughout the startup. If a container exits unexpectedly during startup, Testcontainers for .NET will throw a `ContainerNotRunningException` containing the exit code and logs.

Some containers are intended to stop after completing short-lived tasks like migrations or setup scripts. In these cases, the container exit is expected, not a failure. Use `WaitStrategyMode.OneShot` to treat a normal exit as successful rather than throwing an exception.

```csharp
_ = Wait.ForUnixContainer()
  .UntilMessageIsLogged("Migration completed", o => o.WithMode(WaitStrategyMode.OneShot));
```

## Wait until an HTTP(S) endpoint is available

You can choose to wait for an HTTP(S) endpoint to return a particular HTTP response status code or to match a predicate. The default configuration tries to access the HTTP endpoint running inside the container. Chose `ForPort(ushort)` or `ForPath(string)` to adjust the endpoint or `UsingTls()` to switch to HTTPS. When using `UsingTls()` port 443 is used as a default. If your container exposes a different HTTPS port, make sure that the correct waiting port is configured accordingly.

### Waiting for HTTP response status code _200 OK_ on port 80

```csharp
_ = Wait.ForUnixContainer()
  .UntilHttpRequestIsSucceeded(request => request
    .ForPath("/"));
```

### Waiting for HTTP response status code _200 OK_ or _301 Moved Permanently_

```csharp
_ = Wait.ForUnixContainer()
  .UntilHttpRequestIsSucceeded(request => request
    .ForPath("/")
    .ForStatusCode(HttpStatusCode.OK)
    .ForStatusCode(HttpStatusCode.MovedPermanently));
```

### Waiting for the HTTP response status code to match a predicate

```csharp
_ = Wait.ForUnixContainer()
  .UntilHttpRequestIsSucceeded(request => request
    .ForPath("/")
    .ForStatusCodeMatching(statusCode => statusCode >= HttpStatusCode.OK && statusCode < HttpStatusCode.MultipleChoices));
```

## Wait until a TCP port is available

Testcontainers provides two distinct strategies for waiting until a TCP port becomes available, each serving different purposes depending on your testing needs.

### Wait until an internal TCP port is available

`UntilInternalTcpPortIsAvailable(int)` checks if a service inside the container is listening on the specified port by testing connectivity from within the container itself. This strategy verifies that your application or service has actually started and is ready to accept connections.

```csharp
_ = Wait.ForUnixContainer()
  .UntilInternalTcpPortIsAvailable(8080);
```

!!!note

    Just because a service is listening on the internal TCP port does not necessarily mean it is fully ready to handle requests. Often, wait strategies such as checking for specific log messages or verifying a health endpoint provide more reliable confirmation that the service is operational.

### Wait until an external TCP port is available

`UntilExternalTcpPortIsAvailable(int)` checks if the port is accessible from the test host to the container. This verifies that the port mapping has been established and the port is reachable externally.

```csharp
_ = Wait.ForUnixContainer()
  .UntilExternalTcpPortIsAvailable(8080);
```

!!!note

    External TCP port availability doesn't guarantee that the actual service inside the container is ready to handle requests. It only confirms that the port mapping is established and a connection can be made to the host-side proxy.

## Wait until the container is healthy

If the Docker image supports Dockers's [HEALTHCHECK][docker-docs-healthcheck] feature, like the following configuration:

```Dockerfile
FROM alpine
HEALTHCHECK --interval=1s CMD test -e /healthcheck
```

You can leverage the container's health status as your wait strategy to report readiness of your application or service:

```csharp
_ = new ContainerBuilder("alpine:3.20.0")
  .WithWaitStrategy(Wait.ForUnixContainer().UntilContainerIsHealthy());
```

[docker-docs-healthcheck]: https://docs.docker.com/engine/reference/builder/#healthcheck
