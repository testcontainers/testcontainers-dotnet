
namespace Testcontainers.Pulsar
{
  /// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
  [PublicAPI]
  public sealed class PulsarBuilder : ContainerBuilder<PulsarBuilder, PulsarContainer, PulsarConfiguration>
  {
    public const string PulsarImage = "apachepulsar/pulsar-all:2.11.0";

    public ushort PulsarPort = 6650;
    public ushort PulsarAdminPort = 8080;
    public ushort PulsarSQLPort = 8081;

    public const string StartupScriptFilePath = "/testcontainers.sh";

    /// <summary>
    /// Initializes a new instance of the <see cref="KafkaBuilder" /> class.
    /// </summary>
    public PulsarBuilder()
        : this(new PulsarConfiguration())
    {
      DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KafkaBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private PulsarBuilder(PulsarConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
      DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override PulsarConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override PulsarContainer Build()
    {
      this.Validate();
      return new PulsarContainer(this.DockerResourceConfiguration, TestcontainersSettings.Logger);
    }
    /// <inheritdoc />
    protected override PulsarBuilder Init()
    {
      return base.Init()
          .WithImage(PulsarImage)
          .WithPortBinding(this.PulsarPort, this.PulsarPort)
          .WithPortBinding(this.PulsarAdminPort, this.PulsarAdminPort)
          .WithPortBinding(this.PulsarSQLPort, this.PulsarSQLPort)
          .WithEnvironment("PULSAR_MEM", "-Xms512m -Xmx512m -XX:MaxDirectMemorySize=1g")
          .WithEnvironment("PULSAR_PREFIX_acknowledgmentAtBatchIndexLevelEnabled", "true")
          .WithEnvironment("PULSAR_PREFIX_nettyMaxFrameSizeBytes", "5253120")
          .WithEnvironment("PULSAR_PREFIX_transactionCoordinatorEnabled", "true")
          .WithEnvironment("PULSAR_PREFIX_brokerDeleteInactiveTopicsEnabled", "true")
          .WithEnvironment("PULSAR_STANDALONE_USE_ZOOKEEPER", "1")
          .WithEnvironment("PULSAR_PREFIX_exposingBrokerEntryMetadataToClientEnabled", "true")
          .WithEnvironment("PULSAR_PREFIX_brokerEntryMetadataInterceptors", "org.apache.pulsar.common.intercept.AppendBrokerTimestampMetadataInterceptor,org.apache.pulsar.common.intercept.AppendIndexMetadataInterceptor")

          .WithEntrypoint("/bin/sh", "-c")
          .WithCommand("while [ ! -f " + StartupScriptFilePath + " ]; do sleep 0.1; done; " + StartupScriptFilePath)

          .WithWaitStrategy(Wait.ForUnixContainer())
          .WithStartupCallback((container, ct) =>
          {
            const char lf = '\n';
            var startupScript = new StringBuilder();
            startupScript.Append("#!/bin/bash");
            startupScript.Append(lf);
            startupScript.Append("bin/apply-config-from-env.py conf/standalone.conf ");
            startupScript.Append("&& bin/pulsar standalone --no-functions-worker ");
            startupScript.Append("&& bin/pulsar initialize-transaction-coordinator-metadata -cs localhost:2181 -c standalone --initial-num-transaction-coordinators 2");

            return container.CopyFileAsync(StartupScriptFilePath, Encoding.Default.GetBytes(startupScript.ToString()), 493, ct: ct);
          });
    }

    /// <inheritdoc />
    protected override PulsarBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
      return Merge(this.DockerResourceConfiguration, new PulsarConfiguration(resourceConfiguration));
    }

    protected override PulsarBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
      return Merge(this.DockerResourceConfiguration, new PulsarConfiguration(resourceConfiguration));
    }

    protected override PulsarBuilder Merge(PulsarConfiguration oldValue, PulsarConfiguration newValue)
    {
      return new PulsarBuilder(new PulsarConfiguration(oldValue, newValue));
    }
  }
}
