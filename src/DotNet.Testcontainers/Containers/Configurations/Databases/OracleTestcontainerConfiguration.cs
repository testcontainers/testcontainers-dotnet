namespace DotNet.Testcontainers.Containers.Configurations.Databases
{
  using System;
  using DotNet.Testcontainers.Containers.Configurations.Abstractions;
  using DotNet.Testcontainers.Containers.WaitStrategies;

  public sealed class OracleTestcontainerConfiguration : TestcontainerDatabaseConfiguration
  {
    public OracleTestcontainerConfiguration() : base("wnameless/oracle-xe-11g-r2", 1521)
    {
      // The Dockerfile author did not export $ORACLE_HOME/bin to the global paths. We will use $ORACLE_HOME/bin/sqlplus instead.
      this.Environments["ORACLE_HOME"] = "/u01/app/oracle/product/11.2.0/xe";
    }

    public override string Database
    {
      get => string.Empty;
      set => throw new NotImplementedException();
    }

    public override string Username
    {
      get => "system";
      set => throw new NotImplementedException();
    }

    public override string Password
    {
      get => "oracle";
      set => throw new NotImplementedException();
    }

    public override IWaitForContainerOS WaitStrategy => Wait.ForUnixContainer()
      .UntilCommandIsCompleted($"echo \"exit\" | $ORACLE_HOME/bin/sqlplus -L {this.Username}/{this.Password}@localhost:{this.DefaultPort}/xe | grep Connected > /dev/null");
  }
}
