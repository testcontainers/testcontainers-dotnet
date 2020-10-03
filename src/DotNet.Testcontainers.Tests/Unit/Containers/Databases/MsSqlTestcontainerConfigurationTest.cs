namespace DotNet.Testcontainers.Tests.Unit.Containers.Databases
{
  using System;
  using DotNet.Testcontainers.Containers.Configurations.Databases;
  using Xunit;

  public class MsSqlTestcontainerConfigurationTest
  {
    [Fact]
    public void ThrowExceptionIfPasswordHasLessThen8Characters()
    {
      const string weakPassword = "foo";

      var mssqlConfig = new MsSqlTestcontainerConfiguration();
      Action setPassword = () => mssqlConfig.Password = weakPassword;

      Assert.Throws<Exception>(setPassword);
    }

    [Theory]
    [InlineData("P@ssword123")]
    [InlineData("Password123")]
    [InlineData("P@ssword")]
    [InlineData("p@ssword123")]
    [InlineData("P@SSWORD123")]
    public void NotThrowExceptionIfPasswordIsAValidPassword(string validPassword)
    {
      var mssqlConfig = new MsSqlTestcontainerConfiguration();
      mssqlConfig.Password = validPassword;
      Assert.Equal(validPassword, mssqlConfig.Password);
    }

    [Theory]
    [InlineData("password123")]
    [InlineData("PASSWORD123")]
    [InlineData("p@ssword")]
    [InlineData("P@SSWORD")]
    [InlineData("1234567@")]
    public void ThrowExceptionIfPasswordIsInvalid(string weakPassword)
    {
      var mssqlConfig = new MsSqlTestcontainerConfiguration();

      Action setPassword = () => mssqlConfig.Password = weakPassword;
      Assert.Throws<Exception>(setPassword);
    }
  }
}
