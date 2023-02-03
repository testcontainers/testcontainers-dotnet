# Wait Strategies

Wait strategies are useful to detect if a container is ready for testing (i.e., if the application inside the container is in a usable state). They check different indicators of readiness of the container and complete as soon as they are fulfilled. Per default, Testcontainers will wait until the container is running. For the most images, that is not enough. You can chain different pre-configured wait strategies together or implement your own by implementing the `IWaitUntil` interface.

```csharp
_ = Wait.ForUnixContainer()
  .UntilPortIsAvailable(80)
  .UntilFileExists("/tmp/foo")
  .UntilFileExists("/tmp/bar")
  .UntilOperationIsSucceeded(() => true, 1)
  .AddCustomWaitStrategy(new MyCustomWaitStrategy());
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

## Wait until the container is healthy

If the Docker image supports Dockers's [HEALTHCHECK][docker-docs-healthcheck] feature, like the following configuration:

```Dockerfile
FROM alpine
HEALTHCHECK --interval=1s CMD test -e /healthcheck
```

You can leverage the container's health status as your wait strategy to report readiness of your application or service:

```csharp
_ = new ContainerBuilder()
  .WithWaitStrategy(Wait.ForUnixContainer().UntilContainerIsHealthy());
```

[docker-docs-healthcheck]: https://docs.docker.com/engine/reference/builder/#healthcheck
