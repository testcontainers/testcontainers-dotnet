namespace DotNet.Testcontainers.Configurations
{
  using System;
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  /// <inheritdoc cref="TestcontainerDatabaseConfiguration" />
  [PublicAPI]
  public class OracleTestcontainerConfiguration : TestcontainerDatabaseConfiguration
  {
    private const string OracleImage = "wnameless/oracle-xe-11g-r2";

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
      // The Dockerfile author did not export $ORACLE_HOME/bin to the global paths. We will use $ORACLE_HOME/bin/sqlplus instead.
      this.Environments["ORACLE_HOME"] = "/u01/app/oracle/product/11.2.0/xe";
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
      get => "oracle";
      set => throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override IWaitForContainerOS WaitStrategy => Wait.ForUnixContainer()
      .UntilCommandIsCompleted($"echo \"exit\" | $ORACLE_HOME/bin/sqlplus -L {this.Username}/{this.Password}@localhost:{this.DefaultPort}/xe | grep Connected > /dev/null");
  }
}
