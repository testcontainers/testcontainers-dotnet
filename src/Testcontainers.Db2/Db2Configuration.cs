namespace Testcontainers.Db2;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class Db2Configuration : ContainerConfiguration
{

  /// <summary>
  /// Initializes a new instance of the <see cref="Db2Configuration" /> class.
  /// </summary>
  /// <param name="database">The Db2 database.</param>
  /// <param name="username">The Db2 username.</param>
  /// <param name="password">The Db2 password.</param>
  /// <param name="archiveLogs">The Db2 archive logs setting.</param>
  /// <param name="autoConfig">The Db2 auto config setting.</param>
  /// <param name="licenseAgreement">The Db2 license agreement.</param>
  public Db2Configuration(
        string database = null,
        string username = null,
        string password = null,
        bool archiveLogs = false,
        bool autoConfig = false,
        string licenseAgreement = null)
  {
    Database = database;
    Username = username;
    Password = password;
    ArchiveLogs = archiveLogs;
    AutoConfig = autoConfig;
    LicenseAgreement = licenseAgreement;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Db2Configuration" /> class.
  /// </summary>
  /// <param name="resourceConfiguration">The Docker resource configuration.</param>
  public Db2Configuration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
      : base(resourceConfiguration)
  {
    // Passes the configuration upwards to the base implementations to create an updated immutable copy.
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Db2Configuration" /> class.
  /// </summary>
  /// <param name="resourceConfiguration">The Docker resource configuration.</param>
  public Db2Configuration(IContainerConfiguration resourceConfiguration)
      : base(resourceConfiguration)
  {
    // Passes the configuration upwards to the base implementations to create an updated immutable copy.
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Db2Configuration" /> class.
  /// </summary>
  /// <param name="resourceConfiguration">The Docker resource configuration.</param>
  public Db2Configuration(Db2Configuration resourceConfiguration)
      : this(new Db2Configuration(), resourceConfiguration)
  {
    // Passes the configuration upwards to the base implementations to create an updated immutable copy.
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Db2Configuration" /> class.
  /// </summary>
  /// <param name="oldValue">The old Docker resource configuration.</param>
  /// <param name="newValue">The new Docker resource configuration.</param>
  public Db2Configuration(Db2Configuration oldValue, Db2Configuration newValue)
      : base(oldValue, newValue)
  {
    Database = BuildConfiguration.Combine(oldValue.Database, newValue.Database);
    Username = BuildConfiguration.Combine(oldValue.Username, newValue.Username);
    Password = BuildConfiguration.Combine(oldValue.Password, newValue.Password);
  }

  /// <summary>
  /// Gets the Db2 database.
  /// </summary>
  public string Database { get; }

  /// <summary>
  /// Gets the Db2 username.
  /// </summary>
  public string Username { get; }

  /// <summary>
  /// Gets the Db2 password.
  /// </summary>
  public string Password { get; }

  public bool ArchiveLogs { get; }

  public bool AutoConfig { get; }

  public string LicenseAgreement { get; }
}
