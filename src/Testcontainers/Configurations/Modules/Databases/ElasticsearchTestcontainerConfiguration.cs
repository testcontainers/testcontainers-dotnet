namespace DotNet.Testcontainers.Configurations
{
  using System;
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  /// <inheritdoc cref="TestcontainerDatabaseConfiguration" />
  [PublicAPI]
  public class ElasticsearchTestcontainerConfiguration : TestcontainerDatabaseConfiguration
  {
    private const string ElasticsearchImage = "elasticsearch:8.3.2";

    private const int ElasticsearchPort = 9200;

    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticsearchTestcontainerConfiguration" /> class.
    /// </summary>
    public ElasticsearchTestcontainerConfiguration()
      : this(ElasticsearchImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticsearchTestcontainerConfiguration" /> class.
    /// </summary>
    /// <param name="image">The Docker image.</param>
    public ElasticsearchTestcontainerConfiguration(string image)
      : base(image, ElasticsearchPort)
    {
    }

    /// <inheritdoc />
    public override string Database
    {
      get => string.Empty;
      set => throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override string Username
    {
      get => "elastic";
      set => throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override string Password
    {
      get => this.Environments["ELASTIC_PASSWORD"];
      set => this.Environments["ELASTIC_PASSWORD"] = value;
    }

    /// <inheritdoc />
    public override IWaitForContainerOS WaitStrategy => Wait.ForUnixContainer()
      .UntilPortIsAvailable(this.DefaultPort);
  }
}
