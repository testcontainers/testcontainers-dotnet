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
      var name = info.GetValue<string>("Name");
      var tag = info.GetValue<string>("Tag");
      Image = new DockerImage(repository, name, tag);
    }

    public void Serialize(IXunitSerializationInfo info)
    {
      info.AddValue("Repository", Image.Repository);
      info.AddValue("Name", Image.Name);
      info.AddValue("Tag", Image.Tag);
    }
  }
}
