using System;
using System.Reflection;
using DotNet.Testcontainers.Configurations;
using Xunit.Sdk;

namespace DotNet.Testcontainers.Tests.Unit
{
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Commons;
  using Xunit;
  using Xunit.Abstractions;

  public class DockerConfigTest(ITestOutputHelper output)
  {
    [Fact]
    public void GetCurrentEndpoint()
    {
      AssertCurrentEndpoint();
    }

    [Fact]
    [UseEnvironmentVariable("DOCKER_CONTEXT", "default")]
    public void GetCurrentEndpointWithDefaultDockerContextEnvironmentVariable()
    {
      AssertCurrentEndpoint();
    }

    [Fact]
    [UseEnvironmentVariable("DOCKER_HOST", "tcp://docker:2375")]
    public void GetCurrentEndpointWithDockerHostEnvironmentVariable()
    {
      var endpoint = AssertCurrentEndpoint();
      Assert.Equal(new Uri("tcp://docker:2375"), endpoint);
    }

    private Uri AssertCurrentEndpoint()
    {
      var expectedEndpoint = DockerCli.GetCurrentEndpoint();
      var endpoint = new DockerConfig(new EnvironmentConfiguration()).GetCurrentEndpoint();
      output.WriteLine($"DockerConfig.Default.GetCurrentEndpoint() => {endpoint}");
      Assert.Equal(expectedEndpoint, endpoint);
      Assert.NotNull(endpoint);
      return endpoint;
    }

    [Fact]
    [UseEnvironmentVariable("DOCKER_CONTEXT", "wrong")]
    public void GetCurrentEndpointWithWrongDockerContextEnvironmentVariable()
    {
      var endpoint = new DockerConfig(new EnvironmentConfiguration()).GetCurrentEndpoint();
      Assert.Null(endpoint);
    }
  }

  public class UseEnvironmentVariableAttribute(string variable, string value) : BeforeAfterTestAttribute
  {
    public override void Before(MethodInfo methodUnderTest)
    {
      Environment.SetEnvironmentVariable(variable, value);
    }

    public override void After(MethodInfo methodUnderTest)
    {
      Environment.SetEnvironmentVariable(variable, null);
    }
  }
}
