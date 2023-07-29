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
      var registry = info.GetValue<string>(nameof(IImage.Registry));
      var repository = info.GetValue<string>(nameof(IImage.Repository));
      var tag = info.GetValue<string>(nameof(IImage.Tag));
      Image = new DockerImage(registry, repository, tag);
    }

    public void Serialize(IXunitSerializationInfo info)
    {
      info.AddValue(nameof(IImage.Registry), Image.Registry);
      info.AddValue(nameof(IImage.Repository), Image.Repository);
      info.AddValue(nameof(IImage.Tag), Image.Tag);
    }
  }
}
