namespace DotNet.Testcontainers.Containers.WaitStrategies
{
  using System;
  using System.Collections.Generic;
  using System.IO;

  /// <summary>
  /// Collection of pre-configured strategies to wait until the Testcontainer is up and running.
  /// Waits until all wait strategies are completed.
  /// </summary>
  public interface IWaitForContainerOS
  {
    /// <summary>
    /// Adds a custom wait strategy to the wait strategies collection.
    /// </summary>
    /// <param name="waitStrategy">Wait strategy until the container is ready.</param>
    /// <returns>A configured instance of <see cref="IWaitForContainerOS" />.</returns>
    /// <remarks>Already contains <see cref="DotNet.Testcontainers.Containers.WaitStrategies.Common.UntilContainerIsRunning" /> as default wait strategy.</remarks>
    IWaitForContainerOS AddCustomWaitStrategy(IWaitUntil waitStrategy);

    /// <summary>
    /// Waits until the command is completed successfully.
    /// </summary>
    /// <param name="command">Command to be executed</param>
    /// <returns>A configured instance of <see cref="IWaitForContainerOS" />.</returns>
    /// <remarks>Invokes the operating system command shell. Expects the exit code to be 0.</remarks>
    IWaitForContainerOS UntilCommandIsCompleted(string command);

    /// <summary>
    /// Waits until the command is completed successfully.
    /// </summary>
    /// <param name="command">Command to be executed</param>
    /// <returns>A configured instance of <see cref="IWaitForContainerOS" />.</returns>
    /// <remarks>
    /// Does not invoke the operating system command shell.
    /// Normal shell processing does not happen. Expects the exit code to be 0.
    /// </remarks>
    IWaitForContainerOS UntilCommandIsCompleted(params string[] command);

    /// <summary>
    /// Waits until the file exists.
    /// </summary>
    /// <param name="file">File to be checked.</param>
    /// <returns>A configured instance of <see cref="IWaitForContainerOS" />.</returns>
    IWaitForContainerOS UntilFileExists(string file);

    /// <summary>
    /// Waits until the message is logged in the steam.
    /// </summary>
    /// <param name="stream">Stream to be searched.</param>
    /// <param name="message">Message to be checked.</param>
    /// <returns>A configured instance of <see cref="IWaitForContainerOS" />.</returns>
    IWaitForContainerOS UntilMessageIsLogged(Stream stream, string message);

    /// <summary>
    /// Waits until the operation is completed successfully.
    /// </summary>
    /// <param name="operation">Operation to be executed.</param>
    /// <param name="maxCallCount">Number of attempts before an exception is thrown.</param>
    /// <returns>A configured instance of <see cref="IWaitForContainerOS" />.</returns>
    IWaitForContainerOS UntilOperationIsSucceeded(Func<bool> operation, int maxCallCount);

    /// <summary>
    /// Waits until the port is available.
    /// </summary>
    /// <param name="port">Port to be checked.</param>
    /// <returns>A configured instance of <see cref="IWaitForContainerOS" />.</returns>
    IWaitForContainerOS UntilPortIsAvailable(int port);

    /// <summary>
    /// Returns a collection with all configured wait strategies.
    /// </summary>
    /// <returns>List with all configured wait strategies.</returns>
    IEnumerable<IWaitUntil> Build();
  }
}
