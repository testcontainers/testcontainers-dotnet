namespace DotNet.Testcontainers.Containers.Configurations.Databases
{
  using System;
  using DotNet.Testcontainers.Containers.Configurations.Abstractions;
  using DotNet.Testcontainers.Containers.WaitStrategies;

  public class MsSqlTestcontainerConfiguration : TestcontainerDatabaseConfiguration
  {
    private const string MsSqlImage = "mcr.microsoft.com/mssql/server:2017-CU14-ubuntu";

    private const int MsSqlPort = 1433;

    public MsSqlTestcontainerConfiguration()
      : this(MsSqlImage)
    {
    }

    public MsSqlTestcontainerConfiguration(string image)
      : base(image, MsSqlPort)
    {
      this.Environments["ACCEPT_EULA"] = "Y";
    }

    public override string Database
    {
      get => "master";
      set => throw new NotImplementedException();
    }

    public override string Username
    {
      get => "sa";
      set => throw new NotImplementedException();
    }

    public override string Password
    {
      get => this.Environments["SA_PASSWORD"];
      set => this.Environments["SA_PASSWORD"] = value;
    }

    public override IWaitForContainerOS WaitStrategy => Wait.ForUnixContainer()
      .UntilCommandIsCompleted($"/opt/mssql-tools/bin/sqlcmd -S 'localhost,{this.DefaultPort}' -U '{this.Username}' -P '{this.Password}'");
  }
}
