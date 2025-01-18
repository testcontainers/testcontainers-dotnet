# Custom Configuration

Testcontainers supports various configurations to set up your test environment. It automatically discovers the Docker environment and applies the configuration. You can set or override the default values either with the Testcontainers [properties file][properties-file-format] (`~/.testcontainers.properties`) or with environment variables. The following configurations are available:

| Properties File             | Environment Variable                       | Description                                                                                                               | Default                     |
|-----------------------------|--------------------------------------------|---------------------------------------------------------------------------------------------------------------------------|-----------------------------|
| `docker.config`             | `DOCKER_CONFIG`                            | The directory path that contains the Docker configuration (`config.json`) file.                                           | `~/.docker/`                |
| `docker.host`               | `DOCKER_HOST`                              | The Docker daemon socket to connect to.                                                                                   | -                           |
| `docker.context`            | `DOCKER_CONTEXT`                           | The Docker context to connect to.                                                                                         | -                           |
| `docker.auth.config`        | `DOCKER_AUTH_CONFIG`                       | The Docker configuration file content (GitLab: [Use statically-defined credentials][use-statically-defined-credentials]). | -                           |
| `docker.cert.path`          | `DOCKER_CERT_PATH`                         | The directory path that contains the client certificate (`{ca,cert,key}.pem`) files.                                      | `~/.docker/`                |
| `docker.tls`                | `DOCKER_TLS`                               | Enables TLS.                                                                                                              | `false`                     |
| `docker.tls.verify`         | `DOCKER_TLS_VERIFY`                        | Enables TLS verify.                                                                                                       | `false`                     |
| `host.override`             | `TESTCONTAINERS_HOST_OVERRIDE`             | The host that exposes Docker's ports.                                                                                     | -                           |
| `docker.socket.override`    | `TESTCONTAINERS_DOCKER_SOCKET_OVERRIDE`    | The file path to the Docker daemon socket that is used by Ryuk (resource reaper).                                         | `/var/run/docker.sock`      |
| `ryuk.disabled`             | `TESTCONTAINERS_RYUK_DISABLED`             | Disables Ryuk (resource reaper).                                                                                          | `false`                     |
| `ryuk.container.privileged` | `TESTCONTAINERS_RYUK_CONTAINER_PRIVILEGED` | Runs Ryuk (resource reaper) in privileged mode.                                                                           | `true`                      |
| `ryuk.container.image`      | `TESTCONTAINERS_RYUK_CONTAINER_IMAGE`      | The Ryuk (resource reaper) Docker image.                                                                                  | `testcontainers/ryuk:0.5.1` |
| `hub.image.name.prefix`     | `TESTCONTAINERS_HUB_IMAGE_NAME_PREFIX`     | The name to use for substituting the Docker Hub registry part of the image name.                                          | -                           |
| `wait.strategy.retries`     | `TESTCONTAINERS_WAIT_STRATEGY_RETRIES`     | The wait strategy retry count.                                                                                            | `infinite`                  |
| `wait.strategy.interval`    | `TESTCONTAINERS_WAIT_STRATEGY_INTERVAL`    | The wait strategy interval<sup>1</sup>.                                                                                   | `00:00:01`                  |
| `wait.strategy.timeout`     | `TESTCONTAINERS_WAIT_STRATEGY_TIMEOUT`     | The wait strategy timeout<sup>1</sup>.                                                                                    | `01:00:00`                  |

1) The value represent the string representation of a [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan), for example, `00:00:01` for 1 second.

## Configure remote container runtime

To configure a remote container runtime, Testcontainers provides support for Docker's environment variables in addition to the properties file. During initialization, Testcontainers' auto-discovery feature detect and apply custom configurations including container runtimes. If you are running Docker on a remote host, you can configure it using either of the following methods:

=== "Environment Variable"
    ```
    DOCKER_HOST=tcp://docker:2375
    ```

=== "Properties File"
    ```
    docker.host=tcp://docker:2375
    ```

## Use a different context

You can switch between contexts using the properties file or an environment variable. Once the context is set, Testcontainers will connect to the specified endpoint based on the given value.

```title="List available contexts"
PS C:\Sources\dotnet\testcontainers-dotnet> docker context ls
NAME   DESCRIPTION   DOCKER ENDPOINT           ERROR
tcc                  tcp://127.0.0.1:60706/0
```

Setting the context to `tcc` in this example will use the Docker host running at `127.0.0.1:60706` to create and run the test resources.

=== "Environment Variable"
    ```
    DOCKER_CONTEXT=tcc
    ```

=== "Properties File"
    ```
    docker.context=tcc
    ```

## Enable logging

In .NET logging usually goes through the test framework. Testcontainers is not aware of the project's test framework and may not forward log messages to the appropriate output stream. The default implementation forwards log messages to the `Console` (respectively `stdout` and `stderr`). The output should at least pop up in the IDE running tests in the `Debug` configuration. To override the default implementation, use the builder's `WithLogger(ILogger)` method and provide an `ILogger` instance to replace the default console logger.

    [testcontainers.org 00:00:00.34] Connected to Docker:
      Host: tcp://127.0.0.1:60706/
      Server Version: 70+testcontainerscloud
      Kernel Version: 5.10.137
      API Version: 1.41
      Operating System: Ubuntu 20.04 LTS
      Total Memory: 7.23 GB
    [testcontainers.org 00:00:00.47] Searching Docker registry credential in CredHelpers
    [testcontainers.org 00:00:00.47] Searching Docker registry credential in Auths
    [testcontainers.org 00:00:00.47] Searching Docker registry credential in CredsStore
    [testcontainers.org 00:00:00.47] Searching Docker registry credential in Auths
    [testcontainers.org 00:00:00.51] Docker registry credential https://index.docker.io/v1/ found
    [testcontainers.org 00:00:03.16] Docker image testcontainers/ryuk:0.3.4 created
    [testcontainers.org 00:00:03.24] Docker container b6fad46fbcec84625281c1401ec91158b25cad6495fa612274af7c920abec14e created
    [testcontainers.org 00:00:03.29] Start Docker container b6fad46fbcec84625281c1401ec91158b25cad6495fa612274af7c920abec14e
    [testcontainers.org 00:00:06.18] Docker image alpine:latest created
    [testcontainers.org 00:00:06.22] Docker container 027af397344d08d5fc174bf5b5d449f6b352a8a506306d3d96390aaa2bb0445d created
    [testcontainers.org 00:00:06.26] Start Docker container 027af397344d08d5fc174bf5b5d449f6b352a8a506306d3d96390aaa2bb0445d
    [testcontainers.org 00:00:06.64] Delete Docker container 027af397344d08d5fc174bf5b5d449f6b352a8a506306d3d96390aaa2bb0445d

!!!tip

    These log messages are from the Testcontainers library and contain information about the test resources. They do not include log messages from the containers. To get the container log messages, see: [Getting log messages](https://dotnet.testcontainers.org/api/create_docker_container/#getting-log-messages).

To enable debug log messages in the default implementation, set the property `ConsoleLogger.Instance.DebugLogLevelEnabled` to `true`. This will forward messages related to building or pulling Docker images to the output stream.

[properties-file-format]: https://en.wikipedia.org/wiki/.properties
[use-statically-defined-credentials]: https://docs.gitlab.com/ee/ci/docker/using_docker_images.html#use-statically-defined-credentials
