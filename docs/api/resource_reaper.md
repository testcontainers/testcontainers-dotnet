# Resource Reaper

Testcontainers automatically assigns a Resource Reaper session id to each Docker resource. After the tests are finished — whether they are successful or not — [Moby Ryuk](https://github.com/testcontainers/moby-ryuk) will take care of the remaining Docker resources and removes them. You can change the Resource Reaper session and group Docker resources together. Right now, only Linux containers are supported.

!!! tip

    Whenever possible, do **not** disable the Resource Reaper. It keeps your machine and CI/CD environment clean. If at all, consider disabling the Resource Reaper only for environments that have a mechanism to cleanup Docker resources, e.g. ephemeral CI nodes.

Moby Ryuk derives its name from the anime character [Ryuk](https://en.wikipedia.org/wiki/Ryuk_(Death_Note)) and is a fitting choice due to the intriguing narrative of the anime Death Note.

## Pinned Ryuk image

Testcontainers for .NET pins the Ryuk image to this manifest list (OCI index) digest:

```text
testcontainers/ryuk:0.14.0@sha256:7c1a8a9a47c780ed0f983770a662f80deb115d95cce3e2daa3d12115b8cd28f0
```

If you depend on a private registry, make the image available there either through a registry proxy (pull-through cache) or by copying it from Docker Hub with a tool that preserves the manifest list and all platform variants, for example [`skopeo`](https://github.com/containers/skopeo):

```shell
skopeo copy --all docker://docker.io/testcontainers/ryuk@sha256:7c1a8a9a47c780ed0f983770a662f80deb115d95cce3e2daa3d12115b8cd28f0 docker://mynexus.mydomain/testcontainers/ryuk:0.14.0
```

!!! warning

    Pulling, tagging, and pushing the image manually is not sufficient. That workflow creates a new manifest and only includes the platform variant pulled locally, breaking multi-architecture support.

To use a different image reference, set the `TESTCONTAINERS_RYUK_CONTAINER_IMAGE` environment variable, for example to an unpinned tag:

```shell
TESTCONTAINERS_RYUK_CONTAINER_IMAGE=testcontainers/ryuk:0.14.0
```

<!-- ## Examples

Creates a scoped Resource Reaper and assigns its session id to a container (Docker resource). The container is no longer tracked by the default Resource Reaper.

```csharp
var resourceReaper = await ResourceReaper.GetAndStartNewAsync()
  .ConfigureAwait(false);

await new ContainerBuilder("alpine:3.20.0")
  .WithResourceReaperSessionId(resourceReaper.SessionId)
  .Build()
  .StartAsync()
  .ConfigureAwait(false);
```

!!! warning

    Testcontainers for .NET assigns a default session id. You do not have to override the Resource Reaper session id usually. -->
