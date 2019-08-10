namespace DotNet.Testcontainers.Core.Models.Database
{
  using System.IO;
  using DotNet.Testcontainers.Core.Wait;
  using DotNet.Testcontainers.Diagnostics;

  public sealed class MySqlTestcontainerConfiguration : TestcontainerDatabaseConfiguration
  {
    private const string IsReadyForConnectionMessage = "/usr/sbin/mysqld: ready for connections. .* 3306";

    private readonly Stream outputConsumerStdout = new MemoryStream();

    private readonly Stream outputConsumerStderr = new MemoryStream();

    public MySqlTestcontainerConfiguration() : base("mysql:8.0.15", 3306)
    {
      this.Environments["MYSQL_ALLOW_EMPTY_PASSWORD"] = "yes";
    }

    public override string Database
    {
      get => this.Environments["MYSQL_DATABASE"];
      set => this.Environments["MYSQL_DATABASE"] = value;
    }

    public override string Username
    {
      get => this.Environments["MYSQL_USER"];
      set => this.Environments["MYSQL_USER"] = value;
    }

    public override string Password
    {
      get => this.Environments["MYSQL_PASSWORD"];
      set => this.Environments["MYSQL_PASSWORD"] = value;
    }

    public override IOutputConsumer OutputConsumer => new DefaultConsumer(this.outputConsumerStdout, this.outputConsumerStderr);

    public override IWaitUntil WaitStrategy => Wait.UntilMessagesAreLogged(this.outputConsumerStderr, IsReadyForConnectionMessage);
  }
}
