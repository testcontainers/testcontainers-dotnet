namespace DotNet.Testcontainers
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics.CodeAnalysis;
  using System.Text.RegularExpressions;
  using DotNet.Testcontainers.Images;
  using Microsoft.Extensions.Logging;

  [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1309", MessageId = "Field names should not begin with underscore", Justification = "Do not apply rule for delegate fields.")]
  [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Do not apply rule for delegate fields.")]
  internal static class Logging
  {
    private static readonly Action<ILogger, string, Exception> _Trace
      = LoggerMessage.Define<string>(LogLevel.Trace, default, "{Message}");

    private static readonly Action<ILogger, string, Exception> _Error
      = LoggerMessage.Define<string>(LogLevel.Error, default, "{Message}");

    private static readonly Action<ILogger, Regex, Exception> _IgnorePatternAdded
      = LoggerMessage.Define<Regex>(LogLevel.Information, default, "Pattern {IgnorePattern} added to the regex cache");

    private static readonly Action<ILogger, string, Exception> _DockerContainerCreated
      = LoggerMessage.Define<string>(LogLevel.Information, default, "Docker container {Id} created");

    private static readonly Action<ILogger, string, Exception> _StartDockerContainer
      = LoggerMessage.Define<string>(LogLevel.Information, default, "Start Docker container {Id}");

    private static readonly Action<ILogger, string, Exception> _StopDockerContainer
      = LoggerMessage.Define<string>(LogLevel.Information, default, "Stop Docker container {Id}");

    private static readonly Action<ILogger, string, Exception> _DeleteDockerContainer
      = LoggerMessage.Define<string>(LogLevel.Information, default, "Delete Docker container {Id}");

    private static readonly Action<ILogger, string, string, Exception> _ExtractArchiveToDockerContainer
      = LoggerMessage.Define<string, string>(LogLevel.Information, default, "Copy tar archive to {Path} at Docker container {Id}");

    private static readonly Action<ILogger, string, string, Exception> _GetArchiveFromDockerContainer
      = LoggerMessage.Define<string, string>(LogLevel.Information, default, "Read {Path} from Docker container {Id}");

    private static readonly Action<ILogger, Type, string, Exception> _AttachToDockerContainer
      = LoggerMessage.Define<Type, string>(LogLevel.Information, default, "Attach {OutputConsumer} at Docker container {Id}");

    private static readonly Action<ILogger, IEnumerable<string>, string, Exception> _ExecuteCommandInDockerContainer
      = LoggerMessage.Define<IEnumerable<string>, string>(LogLevel.Information, default, "Execute {Command} at Docker container {Id}");

    private static readonly Action<ILogger, string, Exception> _DockerImageCreated
      = LoggerMessage.Define<string>(LogLevel.Information, default, "Docker image {FullName} created");

    private static readonly Action<ILogger, string, Exception> _DockerImageBuilt
      = LoggerMessage.Define<string>(LogLevel.Information, default, "Docker image {FullName} built");

    private static readonly Action<ILogger, string, Exception> _DeleteDockerImage
      = LoggerMessage.Define<string>(LogLevel.Information, default, "Delete Docker image {FullName}");

    private static readonly Action<ILogger, string, Exception> _DockerNetworkCreated
      = LoggerMessage.Define<string>(LogLevel.Information, default, "Docker network {Id} created");

    private static readonly Action<ILogger, string, Exception> _DeleteDockerNetwork
      = LoggerMessage.Define<string>(LogLevel.Information, default, "Delete Docker network {Id}");

    private static readonly Action<ILogger, string, Exception> _DockerVolumeCreated
      = LoggerMessage.Define<string>(LogLevel.Information, default, "Docker volume {Name} created");

    private static readonly Action<ILogger, string, Exception> _DeleteDockerVolume
      = LoggerMessage.Define<string>(LogLevel.Information, default, "Delete Docker volume {Name}");

    private static readonly Action<ILogger, Guid, Exception> _CanNotGetResourceReaperEndpoint
      = LoggerMessage.Define<Guid>(LogLevel.Debug, default, "Can not get resource reaper {Id} endpoint");

    private static readonly Action<ILogger, Guid, string, Exception> _CanNotConnectToResourceReaper
      = LoggerMessage.Define<Guid, string>(LogLevel.Debug, default, "Can not connect to resource reaper {Id} at {Endpoint}");

    private static readonly Action<ILogger, Guid, string, Exception> _LostConnectionToResourceReaper
      = LoggerMessage.Define<Guid, string>(LogLevel.Debug, default, "Lost connection to resource reaper {Id} at {Endpoint}");

    private static readonly Action<ILogger, string, Exception> _DockerConfigFileNotFound
      = LoggerMessage.Define<string>(LogLevel.Information, default, "Docker config {DockerConfigFilePath} not found");

    private static readonly Action<ILogger, string, Exception> _SearchingDockerRegistryCredential
      = LoggerMessage.Define<string>(LogLevel.Information, default, "Searching Docker registry credential in {CredentialStore}");

    private static readonly Action<ILogger, string, Exception> _DockerRegistryCredentialNotFound
      = LoggerMessage.Define<string>(LogLevel.Information, default, "Docker registry credential {DockerRegistry} not found");

    private static readonly Action<ILogger, string, Exception> _DockerRegistryCredentialFound
      = LoggerMessage.Define<string>(LogLevel.Information, default, "Docker registry credential {DockerRegistry} found");

    public static void Trace(this ILogger logger, string message)
    {
      if (!string.IsNullOrWhiteSpace(message))
      {
        _Trace(logger, message, null);
      }
    }

    public static void Error(this ILogger logger, string message)
    {
      if (!string.IsNullOrWhiteSpace(message))
      {
        _Error(logger, message, null);
      }
    }

    public static void IgnorePatternAdded(this ILogger logger, Regex ignorePattern)
    {
      _IgnorePatternAdded(logger, ignorePattern, null);
    }

    public static void DockerContainerCreated(this ILogger logger, string id)
    {
      _DockerContainerCreated(logger, id, null);
    }

    public static void StartDockerContainer(this ILogger logger, string id)
    {
      _StartDockerContainer(logger, id, null);
    }

    public static void StopDockerContainer(this ILogger logger, string id)
    {
      _StopDockerContainer(logger, id, null);
    }

    public static void DeleteDockerContainer(this ILogger logger, string id)
    {
      _DeleteDockerContainer(logger, id, null);
    }

    public static void ExtractArchiveToDockerContainer(this ILogger logger, string id, string path)
    {
      _ExtractArchiveToDockerContainer(logger, path, id, null);
    }

    public static void GetArchiveFromDockerContainer(this ILogger logger, string id, string path)
    {
      _GetArchiveFromDockerContainer(logger, path, id, null);
    }

    public static void AttachToDockerContainer(this ILogger logger, string id, Type type)
    {
      _AttachToDockerContainer(logger, type, id, null);
    }

    public static void ExecuteCommandInDockerContainer(this ILogger logger, string id, IEnumerable<string> command)
    {
      _ExecuteCommandInDockerContainer(logger, command, id, null);
    }

    public static void DockerImageCreated(this ILogger logger, IDockerImage image)
    {
      _DockerImageCreated(logger, image.FullName, null);
    }

    public static void DockerImageBuilt(this ILogger logger, IDockerImage image)
    {
      _DockerImageBuilt(logger, image.FullName, null);
    }

    public static void DeleteDockerImage(this ILogger logger, IDockerImage image)
    {
      _DeleteDockerImage(logger, image.FullName, null);
    }

    public static void DockerNetworkCreated(this ILogger logger, string id)
    {
      _DockerNetworkCreated(logger, id, null);
    }

    public static void DeleteDockerNetwork(this ILogger logger, string id)
    {
      _DeleteDockerNetwork(logger, id, null);
    }

    public static void DockerVolumeCreated(this ILogger logger, string name)
    {
      _DockerVolumeCreated(logger, name, null);
    }

    public static void DeleteDockerVolume(this ILogger logger, string name)
    {
      _DeleteDockerVolume(logger, name, null);
    }

    public static void CanNotGetResourceReaperEndpoint(this ILogger logger, Guid id, Exception e)
    {
      _CanNotGetResourceReaperEndpoint(logger, id, logger.IsEnabled(LogLevel.Debug) ? e : null);
    }

    public static void CanNotConnectToResourceReaper(this ILogger logger, Guid id, string host, ushort port, Exception e)
    {
      var endpoint = $"{host}:{port}";
      _CanNotConnectToResourceReaper(logger, id, endpoint, e);
    }

    public static void LostConnectionToResourceReaper(this ILogger logger, Guid id, string host, ushort port, Exception e)
    {
      var endpoint = $"{host}:{port}";
      _LostConnectionToResourceReaper(logger, id, endpoint, e);
    }

    public static void DockerConfigFileNotFound(this ILogger logger, string dockerConfigFilePath)
    {
      _DockerConfigFileNotFound(logger, dockerConfigFilePath, null);
    }

    public static void SearchingDockerRegistryCredential(this ILogger logger, string credentialStore)
    {
      _SearchingDockerRegistryCredential(logger, credentialStore, null);
    }

    public static void DockerRegistryCredentialNotFound(this ILogger logger, string dockerRegistry)
    {
      _DockerRegistryCredentialNotFound(logger, dockerRegistry, null);
    }

    public static void DockerRegistryCredentialFound(this ILogger logger, string dockerRegistry)
    {
      _DockerRegistryCredentialFound(logger, dockerRegistry, null);
    }
  }
}
