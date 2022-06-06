namespace DotNet.Testcontainers.Tests.Unit
{
  using System.Text.Json;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Images;
  using Xunit;

  public sealed class DockerRegistryAuthenticationProviderTest
  {
    private const string DockerRegistry = "https://index.docker.io/v1/";

    [Theory]
    [InlineData("baz/foo/bar:1.0.0", null)]
    [InlineData("baz/foo/bar", null)]
    [InlineData("baz/foo/bar:latest", null)]
    [InlineData("foo/bar:1.0.0", null)]
    [InlineData("foo/bar", null)]
    [InlineData("foo/bar:latest", null)]
    [InlineData("bar:1.0.0", null)]
    [InlineData("bar:latest", null)]
    [InlineData("myregistry.azurecr.io/baz/foo/bar:1.0.0", "myregistry.azurecr.io")]
    [InlineData("myregistry.azurecr.io/baz/foo/bar", "myregistry.azurecr.io")]
    [InlineData("myregistry.azurecr.io/baz/foo/bar:latest", "myregistry.azurecr.io")]
    [InlineData("myregistry.azurecr.io/bar:1.0.0", "myregistry.azurecr.io")]
    [InlineData("fedora/httpd:version1.0.test", null)]
    [InlineData("fedora/httpd:version1.0", null)]
    [InlineData("myregistryhost:5000/fedora/httpd:version1.0", "myregistryhost:5000")]
    [InlineData("myregistryhost:5000/httpd:version1.0", "myregistryhost:5000")]
    [InlineData("baz/.foo/bar:1.0.0", null)]
    [InlineData("baz/:foo/bar:1.0.0", null)]
    [InlineData("myregistry.azurecr.io/baz.foo/bar:1.0.0", "myregistry.azurecr.io")]
    [InlineData("myregistry.azurecr.io/baz:foo/bar:1.0.0", "myregistry.azurecr.io")]
    public void GetHostnameFromDockerImage(string dockerImageName, string hostname)
    {
      IDockerImage image = new DockerImage(dockerImageName);
      Assert.Equal(hostname, image.GetHostname());
    }

    [Fact]
    public void ShouldGetDefaultDockerRegistryAuthenticationConfiguration()
    {
      var authenticationProvider = new DockerRegistryAuthenticationProvider("/tmp/docker.config", TestcontainersSettings.Logger);
      Assert.Equal(default(DockerRegistryAuthenticationConfiguration), authenticationProvider.GetAuthConfig(DockerRegistry));
    }

    public sealed class Base64ProviderTest
    {
      [Theory]
      [InlineData("{}", false)]
      [InlineData("{\"auths\":{}}", false)]
      [InlineData("{\"auths\":{\"ghcr.io\":{}}}", true)]
      [InlineData("{\"auths\":{\"" + DockerRegistry + "\":{}}}", true)]
      [InlineData("{\"auths\":{\"" + DockerRegistry + "\":{\"auth\":null}}}", true)]
      [InlineData("{\"auths\":{\"" + DockerRegistry + "\":{\"auth\":\"\"}}}", true)]
      [InlineData("{\"auths\":{\"" + DockerRegistry + "\":{\"auth\":\"dXNlcm5hbWU=\"}}}", true)]
      public void ShouldGetNull(string jsonDocument, bool isApplicable)
      {
        // Given
        var jsonElement = JsonDocument.Parse(jsonDocument).RootElement;

        // When
        var authenticationProvider = new Base64Provider(jsonElement, TestcontainersSettings.Logger);
        var authConfig = authenticationProvider.GetAuthConfig(DockerRegistry);

        // Then
        Assert.Equal(isApplicable, authenticationProvider.IsApplicable());
        Assert.Null(authConfig);
      }

      [Fact]
      public void ShouldGetAuthConfig()
      {
        // Given
        const string jsonDocument = "{\"auths\":{\"" + DockerRegistry + "\":{\"auth\":\"dXNlcm5hbWU6cGFzc3dvcmQ=\"}}}";
        var jsonElement = JsonDocument.Parse(jsonDocument).RootElement;

        // When
        var authenticationProvider = new Base64Provider(jsonElement, TestcontainersSettings.Logger);
        var authConfig = authenticationProvider.GetAuthConfig(DockerRegistry);

        // Then
        Assert.True(authenticationProvider.IsApplicable());
        Assert.NotNull(authConfig);
        Assert.Equal(DockerRegistry, authConfig.RegistryEndpoint);
        Assert.Equal("username", authConfig.Username);
        Assert.Equal("password", authConfig.Password);
      }
    }

    public sealed class CredsStoreProviderTest
    {
      [Theory]
      [InlineData("{}", false)]
      [InlineData("{\"credsStore\":null}", false)]
      [InlineData("{\"credsStore\":\"\"}", false)]
      public void ShouldGetNull(string jsonDocument, bool isApplicable)
      {
        // Given
        var jsonElement = JsonDocument.Parse(jsonDocument).RootElement;

        // When
        var authenticationProvider = new CredsStoreProvider(jsonElement, TestcontainersSettings.Logger);
        var authConfig = authenticationProvider.GetAuthConfig(DockerRegistry);

        // Then
        Assert.Equal(isApplicable, authenticationProvider.IsApplicable());
        Assert.Null(authConfig);
      }

#pragma warning disable xUnit1004

      [Fact(Skip = "The pipeline has no configured credential store (maybe we can use the Windows tests in the future).")]

#pragma warning restore xUnit1004
      public void ShouldGetAuthConfig()
      {
        // Given
        const string jsonDocument = "{\"credsStore\":\"desktop\"}";
        var jsonElement = JsonDocument.Parse(jsonDocument).RootElement;

        // When
        var authenticationProvider = new CredsStoreProvider(jsonElement, TestcontainersSettings.Logger);
        var authConfig = authenticationProvider.GetAuthConfig(DockerRegistry);

        // Then
        Assert.True(authenticationProvider.IsApplicable());
        Assert.NotNull(authConfig);
        Assert.Equal(DockerRegistry, authConfig.RegistryEndpoint);
        Assert.Equal("username", authConfig.Username);
        Assert.Equal("password", authConfig.Password);
      }
    }
  }
}
