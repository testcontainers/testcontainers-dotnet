namespace DotNet.Testcontainers.Containers
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;

  /// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
  [PublicAPI]
  public sealed class SocatBuilder : ContainerBuilder<SocatBuilder, SocatContainer, SocatConfiguration>
  {
    public const string SocatImage = "alpine/socat:1.7.4.3-r0";

    /// <summary>
    /// Initializes a new instance of the <see cref="SocatBuilder" /> class.
    /// </summary>
    public SocatBuilder()
      : this(new SocatConfiguration())
    {
      DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SocatBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private SocatBuilder(SocatConfiguration resourceConfiguration)
      : base(resourceConfiguration)
    {
      DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override SocatConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the Socat target.
    /// </summary>
    /// <param name="exposedPort">The Socat exposed port.</param>
    /// <param name="host">The Socat target host.</param>
    /// <returns>A configured instance of <see cref="SocatBuilder" />.</returns>
    public SocatBuilder WithTarget(int exposedPort, string host)
    {
      return WithTarget(exposedPort, host, exposedPort);
    }

    /// <summary>
    /// Sets the Socat target.
    /// </summary>
    /// <param name="exposedPort">The Socat exposed port.</param>
    /// <param name="host">The Socat target host.</param>
    /// <param name="internalPort">The Socat target port.</param>
    /// <returns>A configured instance of <see cref="SocatBuilder" />.</returns>
    public SocatBuilder WithTarget(int exposedPort, string host, int internalPort)
    {
      var targets = new Dictionary<int, string> { { exposedPort, $"{host}:{internalPort}" } };
      return Merge(DockerResourceConfiguration, new SocatConfiguration(targets))
        .WithPortBinding(exposedPort, true);
    }

    /// <inheritdoc />
    public override SocatContainer Build()
    {
      Validate();

      const string argument = "socat TCP-LISTEN:{0},fork,reuseaddr TCP:{1}";

      var command = string.Join(" & ", DockerResourceConfiguration.Targets
        .Select(item => string.Format(argument, item.Key, item.Value)));

      var waitStrategy = DockerResourceConfiguration.Targets
        .Aggregate(Wait.ForUnixContainer(), (waitStrategy, item) => waitStrategy.UntilPortIsAvailable(item.Key));

      var socatBuilder = WithCommand(command).WithWaitStrategy(waitStrategy);
      return new SocatContainer(socatBuilder.DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override SocatBuilder Init()
    {
      return base.Init()
        .WithImage(SocatImage)
        .WithEntrypoint("/bin/sh", "-c");
    }

    /// <inheritdoc />
    protected override void Validate()
    {
      const string message = "Missing targets. One target must be specified to be created.";

      base.Validate();

      _ = Guard.Argument(DockerResourceConfiguration.Targets, nameof(DockerResourceConfiguration.Targets))
        .ThrowIf(argument => argument.Value.Count == 0, argument => new ArgumentException(message, argument.Name));
    }

    /// <inheritdoc />
    protected override SocatBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
      return Merge(DockerResourceConfiguration, new SocatConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override SocatBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
      return Merge(DockerResourceConfiguration, new SocatConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override SocatBuilder Merge(SocatConfiguration oldValue, SocatConfiguration newValue)
    {
      return new SocatBuilder(new SocatConfiguration(oldValue, newValue));
    }
  }
}
