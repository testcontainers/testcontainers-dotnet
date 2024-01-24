# Resource Reaper

Testcontainers automatically assigns a Resource Reaper session id to each Docker resource. After the tests are finished — whether they are successful or not — [Moby Ryuk](https://github.com/testcontainers/moby-ryuk) will take care of the remaining Docker resources and removes them. You can change the Resource Reaper session and group Docker resources together. Right now, only Linux containers are supported.

!!!tip

    Whenever possible, do **not** disable the Resource Reaper. It keeps your machine and CI/CD environment clean. If at all, consider disabling the Resource Reaper only for environments that have a mechanism to cleanup Docker resources, e.g. ephemeral CI nodes.

Moby Ryuk derives its name from the anime character [Ryuk](https://en.wikipedia.org/wiki/Ryuk_(Death_Note)) and is a fitting choice due to the intriguing narrative of the anime Death Note.

<!-- ## Examples

Creates a scoped Resource Reaper and assigns its session id to a container (Docker resource). The container is no longer tracked by the default Resource Reaper.

```csharp
var resourceReaper = await ResourceReaper.GetAndStartNewAsync()
  .ConfigureAwait(false);

await new ContainerBuilder()
  .WithImage("alpine")
  .WithResourceReaperSessionId(resourceReaper.SessionId)
  .Build()
  .StartAsync()
  .ConfigureAwait(false);
```

!!!warning

    Testcontainers for .NET assigns a default session id. You do not have to override the Resource Reaper session id usually. -->
