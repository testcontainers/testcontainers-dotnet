# Running inside another container

## Docker Desktop

### Sibling containers

If you choose to run your tests in a Docker Wormhole configuration, which involves using sibling containers, it is necessary to mount Docker's raw socket `/var/run/docker.sock.raw`. You find more information and an explanation of the Docker bug in this [comment](https://github.com/docker/for-mac/issues/5588#issuecomment-934600089).

```console
docker run -v /var/run/docker.sock.raw:/var/run/docker.sock $IMAGE dotnet test
```

### Compose

To use Docker's Compose tool to build and run a Testcontainers environment in a Docker Desktop Wormhole configuration,
it is necessary to override Testcontainers' Docker host resolution and set the environment variable `TESTCONTAINERS_HOST_OVERRIDE` to `host.docker.internal`.
Otherwise, Testcontainers cannot access sibling containers like the Resource Reaper Ryuk or other services running on the Docker host.
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
    environment:
      - TESTCONTAINERS_HOST_OVERRIDE=host.docker.internal
    volumes:
      - /var/run/docker.sock:/var/run/docker.sock
```
