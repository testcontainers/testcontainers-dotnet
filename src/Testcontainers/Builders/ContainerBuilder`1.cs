namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Reflection;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;

  /// <summary>
  /// A fluent Docker container builder.
  /// </summary>
  /// <typeparam name="TContainerEntity">The resource entity.</typeparam>
  [PublicAPI]
  [Obsolete("Use the ContainerBuilder class instead. We will remove this builder in the future. Modules are not longer instantiated with reflection. Upcoming modules will have their own specific builder.")]
  public class ContainerBuilder<TContainerEntity> : ContainerBuilder<ContainerBuilder<TContainerEntity>, TContainerEntity, IContainerConfiguration>
    where TContainerEntity : DockerContainer
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ContainerBuilder{TContainerEntity}" /> class.
    /// </summary>
    public ContainerBuilder()
      : this(new ContainerConfiguration())
    {
      var defaultConfiguration = this.Init();
      this.DockerResourceConfiguration = defaultConfiguration.DockerResourceConfiguration;
      this.ModuleConfiguration = defaultConfiguration.ModuleConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContainerBuilder{TContainerEntity}" /> class.
    /// </summary>
    /// <param name="dockerResourceConfiguration">The Docker resource configuration.</param>
    private ContainerBuilder(IContainerConfiguration dockerResourceConfiguration)
      : base(dockerResourceConfiguration)
    {
      this.DockerResourceConfiguration = dockerResourceConfiguration;
    }

    private ContainerBuilder(IContainerConfiguration dockerResourceConfiguration, Action<TContainerEntity> moduleConfiguration)
      : this(dockerResourceConfiguration)
    {
      this.ModuleConfiguration = moduleConfiguration;
    }

    /// <inheritdoc />
    protected override IContainerConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    [Obsolete("Required for extension method module backwards compatibility.")]
    protected override Action<TContainerEntity> ModuleConfiguration { get; }

    /// <inheritdoc />
    public override TContainerEntity Build()
    {
      this.Validate();
      var container = (TContainerEntity)Activator.CreateInstance(typeof(TContainerEntity), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { this.DockerResourceConfiguration, TestcontainersSettings.Logger }, null);
      this.ModuleConfiguration.Invoke(container);
      return container;
    }

    /// <inheritdoc />
    public override ContainerBuilder<TContainerEntity> ConfigureContainer(Action<TContainerEntity> moduleConfiguration)
    {
      return new ContainerBuilder<TContainerEntity>(this.DockerResourceConfiguration, moduleConfiguration ?? this.ModuleConfiguration);
    }

    /// <inheritdoc />
    protected sealed override ContainerBuilder<TContainerEntity> Init()
    {
      return base.Init().ConfigureContainer(container => { });
    }

    /// <inheritdoc />
    protected override ContainerBuilder<TContainerEntity> Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
      return this.Merge(this.DockerResourceConfiguration, new ContainerConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ContainerBuilder<TContainerEntity> Clone(IContainerConfiguration resourceConfiguration)
    {
      return this.Merge(this.DockerResourceConfiguration, new ContainerConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ContainerBuilder<TContainerEntity> Merge(IContainerConfiguration oldValue, IContainerConfiguration newValue)
    {
      return new ContainerBuilder<TContainerEntity>(new ContainerConfiguration(oldValue, newValue), this.ModuleConfiguration);
    }
  }
}
