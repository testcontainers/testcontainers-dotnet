# Low level API access

Testcontainers does not expose all available [Docker Engine APIs](https://docs.docker.com/reference/api/engine/latest/) through its container, image, network, and volume builders. In some cases, you may need to access the underlying Docker Engine API to configure specific properties that are not available via Testcontainers' API. To access all available Docker Engine API properties for creating a resource, use the builder method: `WithCreateParameterModifier(Action<TCreateResourceEntity>)`. This method allows you to use a callback to configure the final payload that is sent to the Docker Engine.

```csharp title="Setting the memory limit to 2GB"
const long TwoGB = 2L * 1024 * 1024 * 1024;
_ = new ContainerBuilder()
  .WithCreateParameterModifier(parameterModifier => parameterModifier.HostConfig.Memory = TwoGB);
```
