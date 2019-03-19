namespace DotNet.Testcontainers.Core.Models.Database
{
  using System;

  public sealed class MsSqlTestcontainerConfiguration : DatabaseConfiguration
  {
    public MsSqlTestcontainerConfiguration() : base("mcr.microsoft.com/mssql/server:2017-CU12-ubuntu", 1433)
    {
      this.environments["ACCEPT_EULA"] = "Y";
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
      get => this.environments["SA_PASSWORD"];
      set => this.environments["SA_PASSWORD"] = value;
    }
  }
}
