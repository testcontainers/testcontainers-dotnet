namespace DotNet.Testcontainers.Configurations
{
  using System.Globalization;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;

  /// <summary>
  /// Waits until a Docker Compose service port accepts connections.
  /// </summary>
  /// <remarks>
  /// The check runs inside the ambassador container that proxies the exposed
  /// service port, not inside the service container. This keeps the readiness
  /// check independent of the service image. A service that does not ship a
  /// shell, such as a distroless image, cannot run a check itself.
  /// </remarks>
  internal sealed class UntilComposeServicePortIsAvailable : IWaitUntil
  {
    /// <summary>
    /// 2 seconds connect timeout. It bounds a single check, the wait strategy
    /// retries until its own timeout is exceeded.
    /// </summary>
    private const int ConnectTimeoutInSeconds = 2;

    private readonly IContainer _ambassadorContainer;

    private readonly string[] _command;

    /// <summary>
    /// Initializes a new instance of the <see cref="UntilComposeServicePortIsAvailable" /> class.
    /// </summary>
    /// <param name="ambassadorContainer">The ambassador container that proxies the exposed service port.</param>
    /// <param name="ipAddress">The IP address of the Docker Compose service container.</param>
    /// <param name="port">The Docker Compose service port.</param>
    public UntilComposeServicePortIsAvailable(IContainer ambassadorContainer, string ipAddress, ushort port)
    {
      _ambassadorContainer = ambassadorContainer;
      _command = new[] { "socat", "-u", "OPEN:/dev/null", string.Format(CultureInfo.InvariantCulture, "TCP:{0}:{1},connect-timeout={2}", ipAddress, port, ConnectTimeoutInSeconds) };
    }

    /// <inheritdoc />
    public async Task<bool> UntilAsync(IContainer container)
    {
      var execResult = await _ambassadorContainer.ExecAsync(_command)
        .ConfigureAwait(false);

      return 0L.Equals(execResult.ExitCode);
    }
  }
}
