namespace DotNet.Testcontainers.Configurations
{
  using System.IO;
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  /// <inheritdoc cref="TestcontainerDatabaseConfiguration" />
  [PublicAPI]
  public class AzureSqlEdgeTestcontainerConfiguration : MsSqlTestcontainerConfiguration
  {
    public override IOutputConsumer OutputConsumer { get; } = Consume.RedirectStdoutAndStderrToStream(new MemoryStream(), new MemoryStream());

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureSqlEdgeTestcontainerConfiguration" /> class.
    /// </summary>
    public AzureSqlEdgeTestcontainerConfiguration()
      : this("mcr.microsoft.com/azure-sql-edge:1.0.6")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureSqlEdgeTestcontainerConfiguration" /> class.
    /// </summary>
    /// <param name="image">
    /// The Docker image.
    /// You can retrieve a list of all available tags for azure-sql-edge at https://mcr.microsoft.com/v2/azure-sql-edge/tags/list
    /// </param>
    public AzureSqlEdgeTestcontainerConfiguration(string image)
      : base(image)
    {
    }

    /// <summary>
    /// Scanning for messages in log since sqlcmd is not available on all platforms:
    ///   https://hub.docker.com/_/microsoft-azure-sql-edge
    ///   https://learn.microsoft.com/en-us/azure/azure-sql-edge/connect#tools-to-connect-to-azure-sql-edge
    /// [!NOTE] sqlcmd tool is not available inside the ARM64 version of SQL Edge containers.
    /// </summary>
    public override IWaitForContainerOS WaitStrategy =>
      Wait
        .ForUnixContainer()
        .UntilMessageIsLogged(this.OutputConsumer.Stdout, "Recovery is complete.*");
  }
}
