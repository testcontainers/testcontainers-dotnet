using DotNet.Testcontainers.Containers;

namespace DotNet.Testcontainers.Configurations
{
  public interface IConnectionStringProvider<TContainerEntity, TConfigurationEntity> where TContainerEntity : IContainer where TConfigurationEntity : IContainerConfiguration
  {
    void Build(TContainerEntity container, TConfigurationEntity configuration);
  }
}
