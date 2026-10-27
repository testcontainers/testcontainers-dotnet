namespace Testcontainers.MiniStack;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class MiniStackConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MiniStackConfiguration" /> class.
    /// </summary>
    /// <param name="initScriptFilePaths">The host file paths of the boot scripts.</param>
    /// <param name="initScriptContents">The file names and contents of the boot scripts.</param>
    /// <param name="readyScriptFilePaths">The host file paths of the ready scripts.</param>
    /// <param name="readyScriptContents">The file names and contents of the ready scripts.</param>
    public MiniStackConfiguration(
        IEnumerable<string> initScriptFilePaths = null,
        IEnumerable<KeyValuePair<string, string>> initScriptContents = null,
        IEnumerable<string> readyScriptFilePaths = null,
        IEnumerable<KeyValuePair<string, string>> readyScriptContents = null)
    {
        InitScriptFilePaths = initScriptFilePaths;
        InitScriptContents = initScriptContents;
        ReadyScriptFilePaths = readyScriptFilePaths;
        ReadyScriptContents = readyScriptContents;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MiniStackConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MiniStackConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MiniStackConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MiniStackConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MiniStackConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MiniStackConfiguration(MiniStackConfiguration resourceConfiguration)
        : this(new MiniStackConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MiniStackConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public MiniStackConfiguration(MiniStackConfiguration oldValue, MiniStackConfiguration newValue)
        : base(oldValue, newValue)
    {
        InitScriptFilePaths = BuildConfiguration.Combine(oldValue.InitScriptFilePaths, newValue.InitScriptFilePaths);
        InitScriptContents = BuildConfiguration.Combine(oldValue.InitScriptContents, newValue.InitScriptContents);
        ReadyScriptFilePaths = BuildConfiguration.Combine(oldValue.ReadyScriptFilePaths, newValue.ReadyScriptFilePaths);
        ReadyScriptContents = BuildConfiguration.Combine(oldValue.ReadyScriptContents, newValue.ReadyScriptContents);
    }

    /// <summary>
    /// Gets the host file paths of the boot scripts.
    /// </summary>
    public IEnumerable<string> InitScriptFilePaths { get; }

    /// <summary>
    /// Gets the file names and contents of the boot scripts.
    /// </summary>
    public IEnumerable<KeyValuePair<string, string>> InitScriptContents { get; }

    /// <summary>
    /// Gets the host file paths of the ready scripts.
    /// </summary>
    public IEnumerable<string> ReadyScriptFilePaths { get; }

    /// <summary>
    /// Gets the file names and contents of the ready scripts.
    /// </summary>
    public IEnumerable<KeyValuePair<string, string>> ReadyScriptContents { get; }
}
