namespace DotNet.Testcontainers.Containers.Configurations.Databases
{
  using System;
  using System.Linq;
  using System.Text.RegularExpressions;
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
      set
      {
        if (!IsPasswordValid(value))
        {
          throw new Exception("Password not valid");
        }

        this.Environments["SA_PASSWORD"] = value;
      }
    }

    public static bool IsPasswordValid(string password)
    {
      var hasUpperChar = new Regex(@"[A-Z]+");
      var hasLowerChar = new Regex(@"[a-z]+");
      var hasNumber = new Regex(@"[0-9]+");
      var appliedRulesCount = 0;

      if (password.Length < 8)
      {
        return false;
      }

      if (hasUpperChar.IsMatch(password))
      {
        appliedRulesCount++;
      }

      if (hasLowerChar.IsMatch(password))
      {
        appliedRulesCount++;
      }

      if (hasNumber.IsMatch(password))
      {
        appliedRulesCount++;
      }

      if (!password.All(c => char.IsLetterOrDigit(c)))
      {
        appliedRulesCount++;
      }

      return appliedRulesCount >= 3;
    }


    public override IWaitForContainerOS WaitStrategy => Wait.ForUnixContainer()
      .UntilCommandIsCompleted($"/opt/mssql-tools/bin/sqlcmd -S 'localhost,{this.DefaultPort}' -U '{this.Username}' -P '{this.Password}'");
  }
}
