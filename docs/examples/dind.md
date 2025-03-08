# Running inside another container

## 'Docker wormhole' pattern - Sibling Docker containers

### Docker-only example

If you choose to run your tests in a Docker Wormhole configuration, which involves using sibling containers, it is necessary to mount Docker's raw socket `/var/run/docker.sock.raw`. You find more information and an explanation of the Docker bug in this [comment](https://github.com/docker/for-mac/issues/5588#issuecomment-934600089).

```shell
docker run -v /var/run/docker.sock.raw:/var/run/docker.sock $IMAGE dotnet test
```

!!! note
    If you are using Docker Desktop, you need to configure the `TESTCONTAINERS_HOST_OVERRIDE` environment variable to use the special DNS name
    `host.docker.internal` for accessing the host from within a container, which is provided by Docker Desktop:
    `-e TESTCONTAINERS_HOST_OVERRIDE=host.docker.internal`

### Docker Compose example

A minimal `docker-compose.yml` file that builds a new container image and runs the test inside the container look something like:

```yaml
version: "3"
services:
  docker_compose_test:
    build:
      dockerfile: Dockerfile
      context: .
    entrypoint: dotnet
    command: test
    # Uncomment the lines below in the case of Docker Desktop (see note above).
    # TESTCONTAINERS_HOST_OVERRIDE is not needed in the case of Docker Engine.
    # environment:
    #   - TESTCONTAINERS_HOST_OVERRIDE=host.docker.internal
    volumes:
      - /var/run/docker.sock:/var/run/docker.sock
```
