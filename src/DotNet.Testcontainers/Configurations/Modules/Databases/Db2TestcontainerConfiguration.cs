namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.IO;
  using DotNet.Testcontainers.Builders;

  public class Db2TestcontainerConfiguration : TestcontainerDatabaseConfiguration
  {
    private const string Db2Image = "ibmcom/db2:11.5.7.0";

    private const int Db2Port = 50000;
    private const string WaitUntilMessageIsLogged = "Setup has completed.";

    /// <summary>
    /// Initializes a new instance of the <see cref="Db2TestcontainerConfiguration" /> class.
    /// </summary>
    public Db2TestcontainerConfiguration()
      : this(Db2Image)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Db2TestcontainerConfiguration" /> class.
    /// </summary>
    /// <param name="image">The Docker image.</param>
    public Db2TestcontainerConfiguration(string image)
      : base(image, Db2Port)
    {
      this.OutputConsumer = Consume.RedirectStdoutAndStderrToStream(new MemoryStream(), new MemoryStream());
      this.WaitStrategy = Wait.ForUnixContainer().UntilMessageIsLogged(this.OutputConsumer.Stdout, WaitUntilMessageIsLogged);
      this.Environments["LICENSE"] = "accept";
      this.Environments["PERSISTENT_HOME"] = "false";
    }

    /// <inheritdoc />
    public override string Database
    {
      get => this.Environments["DBNAME"];
      set => this.Environments["DBNAME"] = value;
    }

    /// <inheritdoc />
    public override string Username
    {
      get => "db2inst1";
      set => throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override string Password
    {
      get => this.Environments["DB2INST1_PASSWORD"];
      set => this.Environments["DB2INST1_PASSWORD"] = value;
    }

    /// <inheritdoc />
    public override IOutputConsumer OutputConsumer { get; }

    /// <inheritdoc />
    public override IWaitForContainerOS WaitStrategy { get; }
  }
}
