# Wait Strategies

Wait strategies are useful to detect if a container is ready for testing (i.e., if the application inside the container is in a usable state). They check different indicators of readiness of the container and complete as soon as they are fulfilled. Per default, Testcontainers will wait until the container is running. For simple images, that is not enough. You can chain different pre-configured wait strategies together or implement your own by implementing the `IWaitUntil` interface.

```csharp
_ = Wait.ForUnixContainer()
  .UntilPortIsAvailable(80)
  .UntilFileExists("/tmp/foo")
  .UntilFileExists("/tmp/bar")
  .UntilOperationIsSucceeded(() => true, 1)
  .AddCustomWaitStrategy(new MyCustomWaitStrategy());
```

## Wait until the container is healthy

If the Docker image supports Dockers's [HEALTHCHECK](docker-docs-healthcheck) feature, like the following configuration:

```Dockerfile
FROM alpine
HEALTHCHECK --interval=1s CMD test -e /healthcheck
```

You can leverage the container's health status as your wait strategy to report readiness of your application or service:

```csharp
_ = new TestcontainersBuilder<TestcontainersContainer>()
  .WithWaitStrategy(Wait.ForUnixContainer().UntilContainerIsHealthy())
  .Build();
```

## Wait until Http Request is successful

You can wait for an HttpResponseCode or even compare the Response Body of an http request to an exposed port on your container

```csharp
_ = new TestcontainersBuilder<TestcontainersContainer>()
  .WithWaitStrategy(
    Wait.ForUnixContainer()
      .UntilHttp() //Default to a GET to localhost:80 [ExposedfromContianer] Expecting 200 with no response body Validation 
  .Build();
```
or with Options 
```csharp
_ = new TestcontainersBuilder<TestcontainersContainer>()
  .WithWaitStrategy(
    Wait.ForUnixContainer()
      .UntilHttp( options => {
        options.Port = [YourServiceExposedPort];
        options.Path = [RequestPath];
        options.Method = [HttpRequestMethod];
        options.ExpectedResponseCodes = new(){ HttpStatusCode.OK, HttpStatusCode.Accepted };
      })
  .Build();
```

[docker-docs-healthcheck]: https://docs.docker.com/engine/reference/builder/#healthcheck

