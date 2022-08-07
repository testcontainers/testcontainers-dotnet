namespace DotNet.Testcontainers.Configurations.Modules.Databases
{
  using System;
  using System.Collections.Generic;
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  public class AzuriteTestcontainerConfiguration : IDisposable
  {
    /// <summary>
    /// Default Azurite docker image.
    /// </summary>
    public const string DefaultAzuriteImage = "mcr.microsoft.com/azure-storage/azurite:3.18.0";

    /// <summary>
    /// Default Blob service listening port. Default is 10000.
    /// </summary>
    public const int DefaultBlobPort = 10000;

    /// <summary>
    /// Default Queue service listening port. Default is 10001.
    /// </summary>
    public const int DefaultQueuePort = 10001;

    /// <summary>
    /// Default Table service listening port. Default 10002.
    /// </summary>
    public const int DefaultTablePort = 10002;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzuriteTestcontainerConfiguration" /> class with default Azurite image.
    /// </summary>
    /// <param name="image">The Docker image.</param>
    public AzuriteTestcontainerConfiguration()
      : this(DefaultAzuriteImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzuriteTestcontainerConfiguration" /> class.
    /// </summary>
    /// <param name="image">The Docker image.</param>
    protected AzuriteTestcontainerConfiguration(string image)
    {
      this.Image = image;
      this.Environments = new Dictionary<string, string>();
      this.OutputConsumer = Consume.DoNotConsumeStdoutAndStderr();
    }

    /// <summary>
    /// Gets the Docker image.
    /// </summary>
    [PublicAPI]
    public string Image { get; }

    /// <summary>
    /// Gets or sets the host Blob port.
    /// </summary>
    /// <remarks>
    /// Corresponds to the default port of the hosted service.
    /// </remarks>
    [PublicAPI]
    public int BlobPort { get; set; }

    /// <summary>
    /// Gets or sets the host Queue port.
    /// </summary>
    /// <remarks>
    /// Corresponds to the default port of the hosted service.
    /// </remarks>
    [PublicAPI]
    public int QueuePort { get; set; }

    /// <summary>
    /// Gets or sets the host Table port.
    /// </summary>
    /// <remarks>
    /// Corresponds to the default port of the hosted service.
    /// </remarks>
    [PublicAPI]
    public int TablePort { get; set; }

    /// <summary>
    /// Gets the environment configuration.
    /// </summary>
    [PublicAPI]
    public IDictionary<string, string> Environments { get; }

    /// <summary>
    /// Gets or sets the output consumer.
    /// </summary>
    /// <remarks>
    /// Uses <see cref="Consume.DoNotConsumeStdoutAndStderr" /> as default value.
    /// </remarks>
    [PublicAPI]
    [CanBeNull]
    public IOutputConsumer OutputConsumer { get; set; }

    /// <summary>
    /// Gets the wait strategy.
    /// </summary>
    /// <remarks>
    /// Uses <see cref="Wait.ForUnixContainer" /> and waits for Azurite ports.
    /// </remarks>
    [PublicAPI]
    public IWaitForContainerOS WaitStrategy => Wait.ForUnixContainer()
      .UntilPortIsAvailable(DefaultBlobPort)
      .UntilPortIsAvailable(DefaultQueuePort)
      .UntilPortIsAvailable(DefaultTablePort);

    /// <inheritdoc />
    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases all resources used by the <see cref="HostedServiceConfiguration" />.
    /// </summary>
    /// <param name="disposing">True if managed resources should be disposed, otherwise false..</param>
    [PublicAPI]
    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.OutputConsumer?.Dispose();
      }
    }
  }
}
