
Pulls `nginx`, creates a new container with port binding `80:80` and hits the default site.

```csharp
var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
  .WithImage("nginx")
  .WithName("nginx")
  .WithPortBinding(80)
  .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(80));
await using (var testcontainers = testcontainersBuilder.Build())
{
  await testcontainers.StartAsync();
  _ = WebRequest.Create("http://localhost:80");
}
```

Mounts the current directory as volume into the container and runs `hostname > /tmp/hostname` on startup.

```csharp
var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
  .WithImage("nginx")
  .WithName("nginx")
  .WithBindMount(".", "/tmp")
  .WithEntrypoint("/bin/sh", "-c")
  .WithCommand("hostname > /tmp/hostname")
  .WithWaitStrategy(Wait.ForUnixContainer().UntilFileExists("/tmp/hostname"));
await using (var testcontainers = testcontainersBuilder.Build())
{
  await testcontainers.StartAsync();
}
```
