# Valkey

[Valkey](https://github.com/valkey-io/valkey) is an open-source, Redis-compatible in-memory data store.

The Testcontainers Redis module is compatible with the Valkey container image. You can use the existing [`Testcontainers.Redis`](/modules/redis/) package without requiring a separate Garnet module.

!!! note

    If Valkey introduces features or configuration options that require dedicated support in the future, we can introduce a separate module at that time.

## Usage

Add the Redis module:

```shell title="NuGet"
dotnet add package Testcontainers.Redis
```

Start a Valkey container using the Redis builder:

```csharp
var valkeyContainer = new RedisBuilder("valkey/valkey:9.0-alpine").Build();
await valkeyContainer.StartAsync();
```

--8<-- "docs/modules/_call_out_test_projects.txt"