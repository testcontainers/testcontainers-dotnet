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

## Wait for healthy container

Container may be running but the application inside not - for example the containerized application is booting. If you define `HEALTHCHECK` in `Dockerfile` it will tell Docker how to test a container to check that it is still working.

```
FROM alpine:3.16
HEALTHCHECK --interval=1s CMD test -e /testfile
...
```

You can use the `UntilContainerIsHealthy` to check that the container is fully ready.

```csharp
_ = new TestcontainersBuilder<TestcontainersContainer>()
  .WithImage(imageName)
  .WithWaitStrategy(Wait.ForUnixContainer().UntilContainerIsHealthy())
  .Build();
```
