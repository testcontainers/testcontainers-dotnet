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
