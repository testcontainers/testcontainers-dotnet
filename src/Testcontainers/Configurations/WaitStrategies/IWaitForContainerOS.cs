namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using JetBrains.Annotations;

  /// <summary>
  /// Collection of pre-configured strategies to wait until the Testcontainer is up and running.
  /// Waits until all wait strategies are completed.
  /// </summary>
  [PublicAPI]
  public interface IWaitForContainerOS
  {
    /// <summary>
    /// Adds a custom wait strategy to the wait strategies collection.
    /// </summary>
    /// <param name="waitStrategy">The wait strategy until the container is ready.</param>
    /// <returns>A configured instance of <see cref="IWaitForContainerOS" />.</returns>
    /// <remarks>Already contains <see cref="UntilContainerIsRunning" /> as default wait strategy.</remarks>
    [PublicAPI]
    IWaitForContainerOS AddCustomWaitStrategy(IWaitUntil waitStrategy);

    /// <summary>
    /// Waits until the command is completed successfully.
    /// </summary>
    /// <param name="command">The command to be executed.</param>
    /// <returns>A configured instance of <see cref="IWaitForContainerOS" />.</returns>
    /// <remarks>Invokes the operating system command shell. Expects the exit code to be 0.</remarks>
    [PublicAPI]
    IWaitForContainerOS UntilCommandIsCompleted(string command);

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
    /// Waits until the file exists.
    /// </summary>
    /// <param name="file">The file to be checked.</param>
    /// <returns>A configured instance of <see cref="IWaitForContainerOS" />.</returns>
    [PublicAPI]
    IWaitForContainerOS UntilFileExists(string file);

    /// <summary>
    /// Waits until the message is logged in the steam.
    /// </summary>
    /// <param name="stream">The stream to be searched.</param>
    /// <param name="message">The message to be checked.</param>
    /// <returns>A configured instance of <see cref="IWaitForContainerOS" />.</returns>
    [PublicAPI]
    IWaitForContainerOS UntilMessageIsLogged(Stream stream, string message);

    /// <summary>
    /// Waits until the operation is completed successfully.
    /// </summary>
    /// <param name="operation">The operation to be executed.</param>
    /// <param name="maxCallCount">The number of attempts before an exception is thrown.</param>
    /// <returns>A configured instance of <see cref="IWaitForContainerOS" />.</returns>
    [PublicAPI]
    IWaitForContainerOS UntilOperationIsSucceeded(Func<bool> operation, int maxCallCount);

    /// <summary>
    /// Waits until the port is available.
    /// </summary>
    /// <param name="port">The port to be checked.</param>
    /// <returns>A configured instance of <see cref="IWaitForContainerOS" />.</returns>
    [PublicAPI]
    IWaitForContainerOS UntilPortIsAvailable(int port);

    /// <summary>
    /// Returns a collection with all configured wait strategies.
    /// </summary>
    /// <returns>List with all configured wait strategies.</returns>
    [PublicAPI]
    IEnumerable<IWaitUntil> Build();
  }
}
