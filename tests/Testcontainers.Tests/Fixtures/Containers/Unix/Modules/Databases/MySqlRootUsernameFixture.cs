namespace DotNet.Testcontainers.Tests.Fixtures
{
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;

  [UsedImplicitly]
  public sealed class MySqlRootUsernameFixture : MySqlFixture
  {
    public MySqlRootUsernameFixture()
      : base(new MySqlTestcontainerConfiguration { Database = "db", Username = "root", Password = "root" })
    {
    }
  }
}
