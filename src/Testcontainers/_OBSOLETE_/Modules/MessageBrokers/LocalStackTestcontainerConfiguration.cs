namespace DotNet.Testcontainers.Configurations
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <inheritdoc cref="TestcontainerMessageBrokerConfiguration" />
  [PublicAPI]
  public class LocalStackTestcontainerConfiguration : TestcontainerMessageBrokerConfiguration
  {
    private const string LocalStackImage = "localstack/localstack:1.2.0";

    private const int LocalStackPort = 4566;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalStackTestcontainerConfiguration" /> class.
    /// </summary>
    public LocalStackTestcontainerConfiguration()
      : this(LocalStackImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalStackTestcontainerConfiguration" /> class.
    /// </summary>
    /// <param name="image">The Docker image.</param>
    public LocalStackTestcontainerConfiguration(string image)
      : base(image, LocalStackPort)
    {
      this.Environments.Add("EXTERNAL_SERVICE_PORTS_START", "4510");
      this.Environments.Add("EXTERNAL_SERVICE_PORTS_END", "4559");
    }

    /// <inheritdoc />
    public override IWaitForContainerOS WaitStrategy => Wait.ForUnixContainer()
      .AddCustomWaitStrategy(new UntilReady());

    private sealed class UntilReady : IWaitUntil
    {
      public async Task<bool> Until(IContainer container, ILogger logger)
      {
        var (stdout, _) = await container.GetLogs()
          .ConfigureAwait(false);
        return stdout != null && stdout.Contains("Ready.\n");
      }
    }
  }
}
