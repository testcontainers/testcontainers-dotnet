namespace DotNet.Testcontainers.Tests.Unit.Containers.Unix.Database
{
  using System;
  using DotNet.Testcontainers.Containers.Configurations.Databases;
  using Xunit;

  public class CouchbaseTestcontainerConfigurationTest
  {
    [Fact]
    public void ShouldNotThrowArgumentOutOfRangeExceptionWhenClusterConfigurationIsValid()
    {
      // Given
      CouchbaseTestcontainerConfiguration configuration = null;

      Action action = () => configuration =  new CouchbaseTestcontainerConfiguration
      {
        BucketName = "Bucket",
        BucketType = "MEMCACHED",
        ClusterRamSize = "1024",
        ClusterIndexRamSize = "512",
        ClusterEventingRamSize = "256",
        ClusterFtsRamSize = "256",
        ClusterAnalyticsRamSize = "1024"
      };

      // When
      var exception = Record.Exception(action.Invoke);

      // Then
      Assert.Null(exception);
      Assert.Equal("Bucket", configuration.BucketName);
      Assert.Equal("MEMCACHED", configuration.BucketType);
      Assert.Equal("1024", configuration.ClusterRamSize);
      Assert.Equal("512", configuration.ClusterIndexRamSize);
      Assert.Equal("256", configuration.ClusterEventingRamSize);
      Assert.Equal("256", configuration.ClusterFtsRamSize);
      Assert.Equal("1024", configuration.ClusterAnalyticsRamSize);
    }

    [Fact]
    public void ShouldThrowArgumentOutOfRangeExceptionWhenClusterRamSizeIsNotValid()
    {
      // Given
      Action action = () => _ = new CouchbaseTestcontainerConfiguration
      {
        ClusterRamSize = "1023"
      };

      // When
      // Then
      var exception = Assert.Throws<ArgumentOutOfRangeException>(action.Invoke);
      Assert.Equal("Couchbase ClusterRamSize ram size can not be less than 1024 MB. (Parameter 'ClusterRamSize')", exception.Message);
    }

    [Fact]
    public void ShouldThrowArgumentOutOfRangeExceptionWhenClusterAnalyticsRamSizeIsNotValid()
    {
      // Given
      Action action = () => _ = new CouchbaseTestcontainerConfiguration
      {
        ClusterAnalyticsRamSize = "1023"
      };

      // When
      // Then
      var exception = Assert.Throws<ArgumentOutOfRangeException>(action.Invoke);
      Assert.Equal("Couchbase ClusterAnalyticsRamSize ram size can not be less than 1024 MB. (Parameter 'ClusterAnalyticsRamSize')", exception.Message);
    }

    [Fact]
    public void ShouldThrowArgumentOutOfRangeExceptionWhenClusterEventingRamSizeIsNotValid()
    {
      // Given
      Action action = () => _ = new CouchbaseTestcontainerConfiguration
      {
        ClusterEventingRamSize = "255"
      };

      // When
      // Then
      var exception = Assert.Throws<ArgumentOutOfRangeException>(action.Invoke);
      Assert.Equal("Couchbase ClusterEventingRamSize ram size can not be less than 256 MB. (Parameter 'ClusterEventingRamSize')", exception.Message);
    }

    [Fact]
    public void ShouldThrowArgumentOutOfRangeExceptionWhenClusterFtsRamSizeIsNotValid()
    {
      // Given
      Action action = () => _ = new CouchbaseTestcontainerConfiguration
      {
        ClusterFtsRamSize = "255"
      };

      // When
      // Then
      var exception = Assert.Throws<ArgumentOutOfRangeException>(action.Invoke);
      Assert.Equal("Couchbase ClusterFtsRamSize ram size can not be less than 256 MB. (Parameter 'ClusterFtsRamSize')", exception.Message);
    }

    [Fact]
    public void ShouldThrowArgumentOutOfRangeExceptionWhenClusterIndexRamSizeIsNotValid()
    {
      // Given
      Action action = () => _ = new CouchbaseTestcontainerConfiguration
      {
        ClusterIndexRamSize = "511"
      };

      // When
      // Then
      var exception = Assert.Throws<ArgumentOutOfRangeException>(action.Invoke);
      Assert.Equal("Couchbase ClusterIndexRamSize ram size can not be less than 512 MB. (Parameter 'ClusterIndexRamSize')", exception.Message);
    }
  }
}
