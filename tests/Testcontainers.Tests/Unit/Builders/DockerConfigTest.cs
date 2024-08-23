using DotNet.Testcontainers.Configurations;

namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Reflection;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Commons;
  using Xunit;
  using Xunit.Abstractions;
  using Xunit.Sdk;

  public sealed class DockerConfigTest
  {
    private readonly ITestOutputHelper _testOutputHelper;

    public DockerConfigTest(ITestOutputHelper testOutputHelper)
    {
      _testOutputHelper = testOutputHelper;
    }

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
      _testOutputHelper.WriteLine($"DockerConfig.Default.GetCurrentEndpoint() => {endpoint}");
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

  public sealed class UseEnvironmentVariableAttribute : BeforeAfterTestAttribute
  {
    private readonly string _variable;

    private readonly string _value;

    public UseEnvironmentVariableAttribute(string variable, string value)
    {
      _variable = variable;
      _value = value;
    }

    public override void Before(MethodInfo methodUnderTest)
    {
      Environment.SetEnvironmentVariable(_variable, _value);
    }

    public override void After(MethodInfo methodUnderTest)
    {
      Environment.SetEnvironmentVariable(_variable, null);
    }
  }
}
