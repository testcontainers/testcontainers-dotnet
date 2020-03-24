namespace DotNet.Testcontainers.Containers.Configurations.Databases
{
  using System;
  using DotNet.Testcontainers.Containers.Configurations.Abstractions;
  using DotNet.Testcontainers.Containers.WaitStrategies;

  public sealed class OracleTestcontainerConfiguration : TestcontainerDatabaseConfiguration
  {
    public OracleTestcontainerConfiguration() : base("wnameless/oracle-xe-11g-r2", 1521)
    {
      this.Environments["ORACLE_ALLOW_EMPTY_PASSWORD"] = "yes";
    }

    public override string Database
    {
      get => this.Environments["ORACLE_DATABASE"];
      set => this.Environments["ORACLE_DATABASE"] = value;
    }

    public override string Username
    {
      get => this.Environments["ORACLE_USER"];
      set => this.Environments["ORACLE_USER"] = value;
    }

    public override string Password
    {
      get => this.Environments["ORACLE_PASSWORD"];
      set => this.Environments["ORACLE_PASSWORD"] = value;
    }

    public override IWaitForContainerOS WaitStrategy => Wait.ForUnixContainer()
      .UntilCommandIsCompleted("bin/bash", "-i", "-c",
        $"echo 'select 123 from dual; exit;' | sqlplus -s {this.Username}/{this.Password}@{this.Database}:{this.Port}/XE | grep -qi '123'");
  }
}
