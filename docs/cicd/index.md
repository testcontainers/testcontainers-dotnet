# Continuous Integration

To use Testcontainers in your CI/CD environment, you only require Docker installed. A local installation of Docker is not mandatory; you can also use a remote Docker installation.

## Azure Pipelines

[Microsoft-hosted agents](https://learn.microsoft.com/en-us/azure/devops/pipelines/agents/hosted?view=azure-devops) come with Docker pre-installed, there is no need for any additional configuration. It is important to note that Windows agents use the Docker Windows engine and cannot run Linux containers. If you are using Windows agents, ensure that the image you are using matches the agent's architecture and operating system version [1)](https://docs.docker.com/build/building/multi-platform/), [2)](https://learn.microsoft.com/en-us/virtualization/windowscontainers/deploy-containers/version-compatibility).

## GitHub Actions

GitHub-hosted runners have the same configuration as Microsoft-hosted agents. The configuration is similar to what is described in the section Azure Pipelines.

## GitLab CI/CD

To configure the Docker service in GitLab CI ([Docker-in-Docker](https://docs.gitlab.com/ee/ci/docker/using_docker_build.html#use-docker-in-docker)), you need to define the service in your `.gitlab-ci.yml` file and expose the Docker host address `docker:2375` by setting the `DOCKER_HOST` environment variable.

```yml title=".gitlab-ci.yml file"
services:
  - docker:dind
variables:
  DOCKER_HOST: tcp://docker:2375
```

## Bitbucket CI/CD

Enable your bitbucket CI/CD pipeline as usual on the admin-pipelines-settings page. After enabling your pipeline, replace the content of *bitbucket-pipelines.yml* (you can find this file in the root of your source folder) with:

```yml
image: mcr.microsoft.com/dotnet/sdk:9.0

options:
  docker: true

pipelines:
  default:
    - step:
        name: Build and Test
        services:
          - docker
        caches:
          - dotnetcore
        script:
          - dotnet restore
          - dotnet build --configuration Release
          - dotnet test --configuration Release --no-build --logger "trx;LogFileName=test-results.trx"
        artifacts:
          - test-results.trx
```

In your code, you can use a [standard hello world xUnit application](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-dotnet-test). Replace the test code with the following code:

```csharp
[Fact]
public async Task Test_HelloWorldContainer()
{
    //https://github.com/testcontainers/testcontainers-dotnet/issues/492
    TestcontainersSettings.ResourceReaperEnabled = false;
    //Arrange
    
    // Create a new instance of a container.
    var container = new ContainerBuilder()
      // Set the image for the container to "testcontainers/helloworld:1.1.0".
      .WithImage("testcontainers/helloworld:1.1.0")
      // Bind port 8080 of the container to a random port on the host.
      .WithPortBinding(8080, true)
      // Wait until the HTTP endpoint of the container is available.
      .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(8080)))
      // Build the container configuration.
      .Build();
    
    await container.StartAsync();

    // Act
    var httpClient = new HttpClient
    {
        BaseAddress = new Uri($"http://{container.Hostname}:{container.GetMappedPublicPort(8080)}")
    };

    var response = await httpClient.GetAsync("/");
    var content = await response.Content.ReadAsStringAsync();

    // Assert
    Assert.True(response.IsSuccessStatusCode, "Expected a successful HTTP response");
    Assert.Contains("Hello world", content, StringComparison.OrdinalIgnoreCase);
}
```

Note the *Reaper* config:

```csharp
TestcontainersSettings.ResourceReaperEnabled = false;
```

If you don't enable this config, the test container will not function correctly.

Run the pipeline by pushing code or by clicking *Run pipeline* on the Pipelines page.
