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
    [Obsolete("Use UntilInternalTcpPortIsAvailable or UntilExternalTcpPortIsAvailable instead. This method corresponds to the internal variant.")]
    IWaitForContainerOS UntilPortIsAvailable(int port, Action<IWaitStrategy> waitStrategyModifier = null);

    /// <summary>
    /// Waits until a TCP port is available from within the container itself.
    /// This verifies that a service inside the container is listening on the specified port.
    /// </summary>
    /// <param name="containerPort">The TCP port of the service running inside the container.</param>
    /// <param name="waitStrategyModifier">The wait strategy modifier to cancel the readiness check.</param>
    /// <returns>A configured instance of <see cref="IWaitForContainerOS" />.</returns>
    [PublicAPI]
    IWaitForContainerOS UntilInternalTcpPortIsAvailable(int containerPort, Action<IWaitStrategy> waitStrategyModifier = null);

    /// <summary>
    /// Waits until a TCP port is available from the test host to the container.
    /// This verifies that the port is exposed and reachable externally.
    /// </summary>
    /// <remarks>
    /// This does not necessarily mean that the TCP connection to the service running inside
    /// the container was successful. For container runtimes like Docker Desktop, Podman, or similar,
    /// this usually only indicates that the port has been mapped and that a connection could be
    /// established to the host-side proxy that maps the port.
    ///
    /// This wait strategy is particularly useful for container runtimes that may take some time
    /// to finish setting up port mappings. In some cases, other strategies such as log-based
    /// readiness checks may indicate readiness before the runtime has fully configured the port
    /// mapping, leading to connection failures. This strategy helps to avoid that race condition.
    /// </remarks>
    /// <param name="containerPort">The TCP port of the service running inside the container.</param>
    /// <param name="waitStrategyModifier">The wait strategy modifier to cancel the readiness check.</param>
    /// <returns>A configured instance of <see cref="IWaitForContainerOS" />.</returns>
    [PublicAPI]
    IWaitForContainerOS UntilExternalTcpPortIsAvailable(int containerPort, Action<IWaitStrategy> waitStrategyModifier = null);

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
