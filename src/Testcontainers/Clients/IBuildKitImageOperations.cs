namespace DotNet.Testcontainers.Clients
{
  using DotNet.Testcontainers.Configurations;

  internal interface IBuildKitImageOperations : IImageBuildOperations<IBuildKitImageFromDockerfileConfiguration>;
}
