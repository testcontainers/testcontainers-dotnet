namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.IO;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  /// <inheritdoc cref="TestcontainerDatabaseConfiguration" />
  [PublicAPI]
  public class ElasticsearchTestcontainerConfiguration : TestcontainerDatabaseConfiguration
  {
    private const string ElasticsearchVmOptionsDirectoryPath = "/usr/share/elasticsearch/config/jvm.options.d";

    private const string ElasticsearchDefaultMemoryVmOptionFileName = "elasticsearch-default-memory-vm.options";

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
      var defaultMemoryVmOption = new DefaultMemoryVmOption();
      this.ResourceMappings[defaultMemoryVmOption.Target] = defaultMemoryVmOption;
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

    /// <inheritdoc cref="IResourceMapping" />
    private sealed class DefaultMemoryVmOption : FileResourceMapping
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="DefaultMemoryVmOption" /> class.
      /// </summary>
      public DefaultMemoryVmOption()
        : base(string.Empty, Path.Combine(ElasticsearchVmOptionsDirectoryPath, ElasticsearchDefaultMemoryVmOptionFileName))
      {
      }

      /// <inheritdoc />
      public override Task<byte[]> GetAllBytesAsync(CancellationToken ct = default)
      {
        return Task.FromResult(Encoding.Default.GetBytes(string.Join("\n", "-Xms2147483648", "-Xmx2147483648")));
      }
    }
  }
}
