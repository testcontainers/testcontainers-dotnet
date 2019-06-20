namespace DotNet.Testcontainers.Core.Models.Database
{
  using System;

  public sealed class MsSqlTestcontainerConfiguration : TestcontainerDatabaseConfiguration
  {
    public MsSqlTestcontainerConfiguration() : base("mcr.microsoft.com/mssql/server:2017-CU14-ubuntu", 1433)
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
  }
}
