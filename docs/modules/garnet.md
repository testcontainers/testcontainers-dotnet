# Garnet

[Garnet](https://github.com/microsoft/garnet) is a high-performance, Redis-compatible in-memory data store.

The Testcontainers Redis module is compatible with the Garnet container image. You can use the existing [`Testcontainers.Redis`](/modules/redis/) package without requiring a separate Garnet module.

!!!note

    If Garnet introduces features or configuration options that require dedicated support in the future, we can introduce a separate module at that time.

## Usage

Add the Redis module:

```shell title="NuGet"
dotnet add package Testcontainers.Redis
```

Start a Garnet container using the Redis builder:

```csharp
var garnetContainer = new RedisBuilder("ghcr.io/microsoft/garnet:1.0.99").Build();
await garnetContainer.StartAsync();
```

--8<-- "docs/modules/_call_out_test_projects.txt"