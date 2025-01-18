namespace DotNet.Testcontainers.Tests.Fixtures
{
  using DotNet.Testcontainers.Images;
  using Xunit.Abstractions;

  public sealed class DockerImageFixtureSerializable : IXunitSerializable
  {
    public DockerImageFixtureSerializable()
    {
    }

    public DockerImageFixtureSerializable(IImage image)
    {
      Image = image;
    }

    public IImage Image { get; private set; }

    public void Deserialize(IXunitSerializationInfo info)
    {
      var repository = info.GetValue<string>("Repository");
      var registry = info.GetValue<string>("Registry");
      var tag = info.GetValue<string>("Tag");
      var digest = info.GetValue<string>("Digest");
      Image = new DockerImage(repository, registry, tag, digest);
    }

    public void Serialize(IXunitSerializationInfo info)
    {
      info.AddValue("Repository", Image.Repository);
      info.AddValue("Registry", Image.Registry);
      info.AddValue("Tag", Image.Tag);
      info.AddValue("Digest", Image.Digest);
    }
  }
}
