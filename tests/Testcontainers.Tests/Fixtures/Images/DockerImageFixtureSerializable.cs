namespace DotNet.Testcontainers.Tests.Fixtures
{
  using DotNet.Testcontainers.Images;
  using Xunit.Abstractions;

  public sealed class DockerImageFixtureSerializable : IXunitSerializable
  {
    public DockerImageFixtureSerializable()
    {
    }

    public DockerImageFixtureSerializable(IDockerImage image)
    {
      this.Image = image;
    }

    public IDockerImage Image { get; private set; }

    public void Deserialize(IXunitSerializationInfo info)
    {
      var repository = info.GetValue<string>("Repository");
      var name = info.GetValue<string>("Name");
      var tag = info.GetValue<string>("Tag");
      this.Image = new DockerImage(repository, name, tag);
    }

    public void Serialize(IXunitSerializationInfo info)
    {
      info.AddValue("Repository", this.Image.Repository);
      info.AddValue("Name", this.Image.Name);
      info.AddValue("Tag", this.Image.Tag);
    }
  }
}
