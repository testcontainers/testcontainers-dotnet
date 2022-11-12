namespace DotNet.Testcontainers.Configurations
{
  using System;
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  /// <inheritdoc cref="TestcontainerDatabaseConfiguration" />
  [PublicAPI]
  public class OracleTestcontainerConfiguration : TestcontainerDatabaseConfiguration
  {
    private const string OracleImage = "gvenzl/oracle-xe:21-slim";

    private const int OraclePort = 1521;

    /// <summary>
    /// Initializes a new instance of the <see cref="OracleTestcontainerConfiguration" /> class.
    /// </summary>
    public OracleTestcontainerConfiguration()
      : this(OracleImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OracleTestcontainerConfiguration" /> class.
    /// </summary>
    /// <param name="image">The Docker image.</param>
    public OracleTestcontainerConfiguration(string image)
      : base(image, OraclePort)
    {
    }

    /// <inheritdoc />
    public override string Database
    {
      get => string.Empty;
      set => throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override string Username
    {
      get => "system";
      set => throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override string Password
    {
      get => this.Environments["ORACLE_PASSWORD"];
      set => this.Environments["ORACLE_PASSWORD"] = value;
    }

    /// <inheritdoc />
    public override IWaitForContainerOS WaitStrategy => Wait.ForUnixContainer()
      .UntilCommandIsCompleted($"echo \"exit\" | $ORACLE_HOME/bin/sqlplus -L {this.Username}/{this.Password}@localhost:{this.DefaultPort}/xe | grep Connected > /dev/null");
  }
}
