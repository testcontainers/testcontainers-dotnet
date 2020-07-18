namespace DotNet.Testcontainers.Tests.Unit.Containers.Unix.Database
{
  using System;
  using Testcontainers.Containers.Configurations.Databases;
  using Xunit;

  public class CouchbaseTestcontainerConfigurationTest
  {
    [Fact]
    public void ShouldNotThrowsArgumentOutOfRangeException_WhenClusterConfigurationIsValid()
    {
      //Given
      Action action = () => new CouchbaseTestcontainerConfiguration
      {
        ClusterRamSize = "1024",
        ClusterIndexRamSize = "512",
        ClusterEventingRamSize = "256",
        ClusterFtsRamSize = "256",
        ClusterAnalyticsRamSize = "1024"
      };
      //When
      var exception = Record.Exception(action);
      //Then
      Assert.Null(exception);
    }

    [Fact]
    public void ShouldThrowsArgumentOutOfRangeException_WhenClusterRamSizeIsNotValid()
    {
      //Given && When
      Func<CouchbaseTestcontainerConfiguration> action = () => new CouchbaseTestcontainerConfiguration
      {
        ClusterRamSize = "1023"
      };
      //Then
      var exception = Assert.Throws<ArgumentOutOfRangeException>(action);
      Assert.Equal("Couchbase ClusterRamSize ram size can not be less than 1024 MB. (Parameter 'ClusterRamSize')",exception.Message);
    }

    [Fact]
    public void ShouldThrowsArgumentOutOfRangeException_WhenClusterAnalyticsRamSizeIsNotValid()
    {
      //Given && When
      Func<CouchbaseTestcontainerConfiguration> action = () => new CouchbaseTestcontainerConfiguration
      {
        ClusterAnalyticsRamSize = "1023"
      };
      //Then
      var exception = Assert.Throws<ArgumentOutOfRangeException>(action);
      Assert.Equal("Couchbase ClusterAnalyticsRamSize ram size can not be less than 1024 MB. (Parameter 'ClusterAnalyticsRamSize')",exception.Message);
    }

    [Fact]
    public void ShouldThrowsArgumentOutOfRangeException_WhenClusterEventingRamSizeIsNotValid()
    {
      //Given && When
      Func<CouchbaseTestcontainerConfiguration> action = () => new CouchbaseTestcontainerConfiguration
      {
        ClusterEventingRamSize = "255"
      };
      //Then
      var exception = Assert.Throws<ArgumentOutOfRangeException>(action);
      Assert.Equal("Couchbase ClusterEventingRamSize ram size can not be less than 256 MB. (Parameter 'ClusterEventingRamSize')",exception.Message);
    }

    [Fact]
    public void ShouldThrowsArgumentOutOfRangeException_WhenClusterFtsRamSizeIsNotValid()
    {
      //Given && When
      Func<CouchbaseTestcontainerConfiguration> action = () => new CouchbaseTestcontainerConfiguration
      {
        ClusterFtsRamSize = "255"
      };
      //Then
      var exception = Assert.Throws<ArgumentOutOfRangeException>(action);
      Assert.Equal("Couchbase ClusterFtsRamSize ram size can not be less than 256 MB. (Parameter 'ClusterFtsRamSize')", exception.Message);
    }

    [Fact]
    public void ShouldThrowsArgumentOutOfRangeException_WhenClusterIndexRamSizeIsNotValid()
    {
      //Given && When
      Func<CouchbaseTestcontainerConfiguration> action = () => new CouchbaseTestcontainerConfiguration
      {
        ClusterIndexRamSize = "511"
      };
      //Then
      var exception = Assert.Throws<ArgumentOutOfRangeException>(action);
      Assert.Equal("Couchbase ClusterIndexRamSize ram size can not be less than 512 MB. (Parameter 'ClusterIndexRamSize')",exception.Message);
    }

  }
}
