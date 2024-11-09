namespace DotNet.Testcontainers.Tests.Fixtures
{
  using DotNet.Testcontainers.Images;
  using Xunit;

  public sealed class DockerImageFixture : TheoryData<DockerImageFixtureSerializable, string, string>
  {
    private const string FooBarBaz = "foo/bar/baz";
    private const string BarBaz = "bar/baz";
    private const string Baz = "baz";
    private const string FedoraHttpd = "fedora/httpd";
    private const string LatestTag = "latest";
    private const string SemVerTag = "1.0.0";
    private const string CustomTag1 = "version1.0";
    private const string CustomTag2 = "version1.0.test";
    private const string DotSeparatorRegistry = "myregistry.azurecr.io";
    private const string PortSeparatorRegistry = "myregistry:5000";
    private const string Digest = "sha256:aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
    private const string HubImageNamePrefixImplicitLibrary = "myregistry.azurecr.io";
    private const string HubImageNamePrefixExplicitLibrary = "myregistry.azurecr.io/library";

    public DockerImageFixture()
    {
      Add(new DockerImageFixtureSerializable(new DockerImage(FooBarBaz, null, SemVerTag, null)), $"{FooBarBaz}:{SemVerTag}", $"{FooBarBaz}:{SemVerTag}");
      Add(new DockerImageFixtureSerializable(new DockerImage(FooBarBaz, null, null, null)), FooBarBaz, $"{FooBarBaz}:{LatestTag}");
      Add(new DockerImageFixtureSerializable(new DockerImage(FooBarBaz, null, null, null)), $"{FooBarBaz}:{LatestTag}", $"{FooBarBaz}:{LatestTag}");
      Add(new DockerImageFixtureSerializable(new DockerImage(BarBaz, null, SemVerTag, null)), $"{BarBaz}:{SemVerTag}", $"{BarBaz}:{SemVerTag}");
      Add(new DockerImageFixtureSerializable(new DockerImage(BarBaz, null, null, null)), BarBaz, $"{BarBaz}:{LatestTag}");
      Add(new DockerImageFixtureSerializable(new DockerImage(BarBaz, null, null, null)), $"{BarBaz}:{LatestTag}", $"{BarBaz}:{LatestTag}");
      Add(new DockerImageFixtureSerializable(new DockerImage(Baz, null, SemVerTag, null)), $"{Baz}:{SemVerTag}", $"{Baz}:{SemVerTag}");
      Add(new DockerImageFixtureSerializable(new DockerImage(Baz, null, null, null)), $"{Baz}:{LatestTag}", $"{Baz}:{LatestTag}");
      Add(new DockerImageFixtureSerializable(new DockerImage(FooBarBaz, DotSeparatorRegistry, SemVerTag, null)), $"{DotSeparatorRegistry}/{FooBarBaz}:{SemVerTag}", $"{DotSeparatorRegistry}/{FooBarBaz}:{SemVerTag}");
      Add(new DockerImageFixtureSerializable(new DockerImage(FooBarBaz, DotSeparatorRegistry, null, null)), $"{DotSeparatorRegistry}/{FooBarBaz}", $"{DotSeparatorRegistry}/{FooBarBaz}:{LatestTag}");
      Add(new DockerImageFixtureSerializable(new DockerImage(FooBarBaz, DotSeparatorRegistry, null, null)), $"{DotSeparatorRegistry}/{FooBarBaz}:{LatestTag}", $"{DotSeparatorRegistry}/{FooBarBaz}:{LatestTag}");
      Add(new DockerImageFixtureSerializable(new DockerImage(FedoraHttpd, null, CustomTag1, null)), $"{FedoraHttpd}:{CustomTag1}", $"{FedoraHttpd}:{CustomTag1}");
      Add(new DockerImageFixtureSerializable(new DockerImage(FedoraHttpd, null, CustomTag2, null)), $"{FedoraHttpd}:{CustomTag2}", $"{FedoraHttpd}:{CustomTag2}");
      Add(new DockerImageFixtureSerializable(new DockerImage(FedoraHttpd, PortSeparatorRegistry, CustomTag1, null)), $"{PortSeparatorRegistry}/{FedoraHttpd}:{CustomTag1}", $"{PortSeparatorRegistry}/{FedoraHttpd}:{CustomTag1}");
      Add(new DockerImageFixtureSerializable(new DockerImage(FooBarBaz, DotSeparatorRegistry, SemVerTag, Digest)), $"{DotSeparatorRegistry}/{FooBarBaz}:{SemVerTag}@{Digest}", $"{DotSeparatorRegistry}/{FooBarBaz}:{SemVerTag}@{Digest}");
      Add(new DockerImageFixtureSerializable(new DockerImage(FooBarBaz, DotSeparatorRegistry, null, Digest)), $"{DotSeparatorRegistry}/{FooBarBaz}@{Digest}", $"{DotSeparatorRegistry}/{FooBarBaz}@{Digest}");
      Add(new DockerImageFixtureSerializable(new DockerImage(FooBarBaz, null, null, null, HubImageNamePrefixImplicitLibrary)), $"{HubImageNamePrefixImplicitLibrary}/{FooBarBaz}", $"{HubImageNamePrefixImplicitLibrary}/{FooBarBaz}:{LatestTag}");
      Add(new DockerImageFixtureSerializable(new DockerImage(FooBarBaz, null, null, null, HubImageNamePrefixExplicitLibrary)), $"{HubImageNamePrefixExplicitLibrary}/{FooBarBaz}", $"{HubImageNamePrefixExplicitLibrary}/{FooBarBaz}:{LatestTag}");
    }
  }
}
