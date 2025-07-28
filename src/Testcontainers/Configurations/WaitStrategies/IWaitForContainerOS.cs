namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using System.Data.Common;
  using System.Text.RegularExpressions;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;

  /// <summary>
  /// Collection of pre-configured strategies to wait until the container is up and running.
  /// </summary>
  [PublicAPI]
  public interface IWaitForContainerOS
  {
    /// <summary>
    /// Adds a custom wait strategy to the wait strategies collection.
    /// </summary>
    /// <param name="waitUntil">The wait strategy until the container is ready.</param>
    /// <param name="waitStrategyModifier">The wait strategy modifier to cancel the readiness check.</param>
    /// <returns>A configured instance of <see cref="IWaitForContainerOS" />.</returns>
    /// <remarks>Already contains <see cref="UntilContainerIsRunning" /> as default wait strategy.</remarks>
    [PublicAPI]
    IWaitForContainerOS AddCustomWaitStrategy(IWaitUntil waitUntil, Action<IWaitStrategy> waitStrategyModifier = null);

    /// <summary>
    /// Waits until the command is completed successfully.
    /// </summary>
    /// <param name="command">The command to be executed.</param>
    /// <returns>A configured instance of <see cref="IWaitForContainerOS" />.</returns>
    /// <remarks>
    /// Does not invoke the operating system command shell.
    /// Normal shell processing does not happen. Expects the exit code to be 0.
    /// </remarks>
    [PublicAPI]
    IWaitForContainerOS UntilCommandIsCompleted(params string[] command);

    /// <summary>
    /// Waits until the command is completed successfully.
    /// </summary>
    /// <param name="command">The command to be executed.</param>
    /// <param name="waitStrategyModifier">The wait strategy modifier to cancel the readiness check.</param>
    /// <returns>A configured instance of <see cref="IWaitForContainerOS" />.</returns>
    /// <remarks>Invokes the operating system command shell. Expects the exit code to be 0.</remarks>
    [PublicAPI]
    IWaitForContainerOS UntilCommandIsCompleted(string command, Action<IWaitStrategy> waitStrategyModifier = null);

    /// <summary>
    /// Waits until the command is completed successfully.
    /// </summary>
    /// <param name="command">The command to be executed.</param>
    /// <param name="waitStrategyModifier">The wait strategy modifier to cancel the readiness check.</param>
    /// <returns>A configured instance of <see cref="IWaitForContainerOS" />.</returns>
    /// <remarks>
    /// Does not invoke the operating system command shell.
    /// Normal shell processing does not happen. Expects the exit code to be 0.
    /// </remarks>
    [PublicAPI]
    IWaitForContainerOS UntilCommandIsCompleted(IEnumerable<string> command, Action<IWaitStrategy> waitStrategyModifier = null);

    /// <summary>
    /// Waits until the port is available.
    /// </summary>
    /// <param name="port">The port to be checked.</param>
    /// <param name="waitStrategyModifier">The wait strategy modifier to cancel the readiness check.</param>
    /// <returns>A configured instance of <see cref="IWaitForContainerOS" />.</returns>
    [PublicAPI]
    IWaitForContainerOS UntilPortIsAvailable(int port, Action<IWaitStrategy> waitStrategyModifier = null);

    /// <summary>
    /// Waits until the file exists.
    /// </summary>
    /// <param name="filePath">The file path to be checked.</param>
    /// <param name="fileSystem">The file system to be checked.</param>
    /// <param name="waitStrategyModifier">The wait strategy modifier to cancel the readiness check.</param>
    /// <returns>A configured instance of <see cref="IWaitForContainerOS" />.</returns>
    [PublicAPI]
    IWaitForContainerOS UntilFileExists(string filePath, FileSystem fileSystem = FileSystem.Host, Action<IWaitStrategy> waitStrategyModifier = null);

    /// <summary>
    /// Waits until the message is logged.
    /// </summary>
    /// <param name="pattern">The regular expression that matches the log message.</param>
    /// <param name="waitStrategyModifier">The wait strategy modifier to cancel the readiness check.</param>
    /// <returns>A configured instance of <see cref="IWaitForContainerOS" />.</returns>
    [PublicAPI]
    IWaitForContainerOS UntilMessageIsLogged(string pattern, Action<IWaitStrategy> waitStrategyModifier = null);

    /// <summary>
    /// Waits until the message is logged.
    /// </summary>
    /// <param name="pattern">The regular expression that matches the log message.</param>
    /// <param name="waitStrategyModifier">The wait strategy modifier to cancel the readiness check.</param>
    /// <returns>A configured instance of <see cref="IWaitForContainerOS" />.</returns>
    [PublicAPI]
    IWaitForContainerOS UntilMessageIsLogged(Regex pattern, Action<IWaitStrategy> waitStrategyModifier = null);

    /// <summary>
    /// Waits until the tcp connection is established successfully from host.
    /// </summary>
    /// <remarks>
    /// Some docker configurations (DockerForMac & Rancher on Mac & Windows) allow establishing a connection to any mapped port.
    /// The test will always succeed in these cases.
    /// </remarks>
    /// <param name="port">The container port to be checked.</param>
    /// <param name="waitStrategyModifier">The wait strategy modifier to cancel the readiness check.</param>
    /// <returns>A configured instance of <see cref="IWaitForContainerOS" />.</returns>
    [PublicAPI]
    IWaitForContainerOS UntilTcpConnected(int port, Action<IWaitStrategy> waitStrategyModifier = null);

    /// <summary>
    /// Waits until the http request is completed successfully.
    /// </summary>
    /// <param name="request">The http request to be executed.</param>
    /// <param name="waitStrategyModifier">The wait strategy modifier to cancel the readiness check.</param>
    /// <returns>A configured instance of <see cref="IWaitForContainerOS" />.</returns>
    [PublicAPI]
    IWaitForContainerOS UntilHttpRequestIsSucceeded(Func<HttpWaitStrategy, HttpWaitStrategy> request, Action<IWaitStrategy> waitStrategyModifier = null);

    /// <summary>
    /// Waits until the container is healthy.
    /// </summary>
    /// <param name="failingStreak">The number of attempts before an exception is thrown.</param>
    /// <param name="waitStrategyModifier">The wait strategy modifier to cancel the readiness check.</param>
    /// <returns>A configured instance of <see cref="IWaitForContainerOS" />.</returns>
    /// <exception cref="TimeoutException">Thrown when number of failed operations exceeded <paramref name="failingStreak" />.</exception>
    [PublicAPI]
    IWaitForContainerOS UntilContainerIsHealthy(long failingStreak = 3, Action<IWaitStrategy> waitStrategyModifier = null);

    /// <summary>
    /// Waits until a successful connection to the database can be established.
    /// </summary>
    /// <remarks>
    /// To use this wait strategy, the container must implement the <see cref="IDatabaseContainer" /> interface.
    /// </remarks>
    /// <param name="dbProviderFactory">The <see cref="DbProviderFactory" /> used to create the database connection.</param>
    /// <param name="waitStrategyModifier">The wait strategy modifier to cancel the readiness check.</param>
    /// <returns>A configured instance of <see cref="IWaitForContainerOS" />.</returns>
    [PublicAPI]
    IWaitForContainerOS UntilDatabaseIsAvailable(DbProviderFactory dbProviderFactory, Action<IWaitStrategy> waitStrategyModifier = null);

    /// <summary>
    /// Returns a collection with all configured wait strategies.
    /// </summary>
    /// <returns>Returns a list with all configured wait strategies.</returns>
    [PublicAPI]
    IEnumerable<WaitStrategy> Build();
  }
}
