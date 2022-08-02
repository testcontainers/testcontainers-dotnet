namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Text.Json;
  using DotNet.Testcontainers.Images;

  /// <summary>
  /// Reads and maps the custom configurations from the Testcontainers properties file.
  /// </summary>
  internal sealed class PropertiesFileConfiguration : CustomConfiguration, ICustomConfiguration
  {
    static PropertiesFileConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertiesFileConfiguration" /> class.
    /// </summary>
    public PropertiesFileConfiguration()
      : this(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".testcontainers.properties"))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertiesFileConfiguration" /> class.
    /// </summary>
    /// <param name="propertiesFilePath">The Java properties file path.</param>
    public PropertiesFileConfiguration(string propertiesFilePath)
      : this(File.Exists(propertiesFilePath)
        ? File.ReadAllLines(propertiesFilePath)
        : Array.Empty<string>())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertiesFileConfiguration" /> class.
    /// </summary>
    /// <param name="lines">A list of Java properties file lines.</param>
    public PropertiesFileConfiguration(params string[] lines)
      : base(lines
        .Select(line => line.Trim())
        .Where(line => !string.IsNullOrEmpty(line))
        .Where(line => !line.StartsWith("#", StringComparison.Ordinal))
        .Where(line => !line.StartsWith("!", StringComparison.Ordinal))
        .Select(line => line.Split(new[] { '=', ':', ' ' }, 2, StringSplitOptions.RemoveEmptyEntries))
        .Where(property => 2.Equals(property.Length))
        .ToDictionary(property => property[0], property => property[1]))
    {
    }

    /// <summary>
    /// Gets the <see cref="ICustomConfiguration" /> instance.
    /// </summary>
    public static ICustomConfiguration Instance { get; }
      = new PropertiesFileConfiguration();

    /// <inheritdoc />
    public string GetDockerConfig()
    {
      const string propertyName = "docker.config";
      return this.GetDockerConfig(propertyName);
    }

    /// <inheritdoc />
    public Uri GetDockerHost()
    {
      const string propertyName = "docker.host";
      return this.GetDockerHost(propertyName);
    }

    /// <inheritdoc />
    public JsonDocument GetDockerAuthConfig()
    {
      const string propertyName = "docker.auth.config";
      return this.GetDockerAuthConfig(propertyName);
    }

    /// <inheritdoc />
    public bool GetRyukDisabled()
    {
      const string propertyName = "ryuk.disabled";
      return this.GetRyukDisabled(propertyName);
    }

    /// <inheritdoc />
    public IDockerImage GetRyukContainerImage()
    {
      const string propertyName = "ryuk.container.image";
      return this.GetRyukContainerImage(propertyName);
    }

    /// <inheritdoc />
    public string GetHubImageNamePrefix()
    {
      const string propertyName = "hub.image.name.prefix";
      return this.GetHubImageNamePrefix(propertyName);
    }
  }
}
