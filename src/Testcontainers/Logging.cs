namespace DotNet.Testcontainers
{
  using System;
  using System.Collections.Generic;
  using System.Text.Json;
  using System.Text.RegularExpressions;
  using DotNet.Testcontainers.Images;
  using Microsoft.Extensions.Logging;

  internal static class Logging
  {
#pragma warning disable InconsistentNaming, SA1309

    private static readonly Action<ILogger, Regex, Exception> _IgnorePatternAdded
      = LoggerMessage.Define<Regex>(LogLevel.Information, default, "Pattern {IgnorePattern} added to the regex cache");

    private static readonly Action<ILogger, string, Exception> _DockerContainerCreated
      = LoggerMessage.Define<string>(LogLevel.Information, default, "Docker container {Id} created");

    private static readonly Action<ILogger, string, Exception> _StartDockerContainer
      = LoggerMessage.Define<string>(LogLevel.Information, default, "Start Docker container {Id}");

    private static readonly Action<ILogger, string, Exception> _StopDockerContainer
      = LoggerMessage.Define<string>(LogLevel.Information, default, "Stop Docker container {Id}");

    private static readonly Action<ILogger, string, Exception> _PauseDockerContainer
      = LoggerMessage.Define<string>(LogLevel.Information, default, "Pause Docker container {Id}");

    private static readonly Action<ILogger, string, Exception> _UnpauseDockerContainer
      = LoggerMessage.Define<string>(LogLevel.Information, default, "Unpause Docker container {Id}");

    private static readonly Action<ILogger, string, Exception> _DeleteDockerContainer
      = LoggerMessage.Define<string>(LogLevel.Information, default, "Delete Docker container {Id}");

    private static readonly Action<ILogger, string, Exception> _StartReadinessCheck
      = LoggerMessage.Define<string>(LogLevel.Information, default, "Wait for Docker container {Id} to complete readiness checks");

    private static readonly Action<ILogger, string, Exception> _CompleteReadinessCheck
      = LoggerMessage.Define<string>(LogLevel.Information, default, "Docker container {Id} ready");

    private static readonly Action<ILogger, long, string, Exception> _CopyArchiveToDockerContainer
      = LoggerMessage.Define<long, string>(LogLevel.Information, default, "Copy tar archive to container: Content length: {Length} byte(s), Docker container: {Id}");

    private static readonly Action<ILogger, string, string, Exception> _ReadArchiveFromDockerContainer
      = LoggerMessage.Define<string, string>(LogLevel.Information, default, "Read \"{Path}\" from Docker container {Id}");

    private static readonly Action<ILogger, Type, string, Exception> _AttachToDockerContainer
      = LoggerMessage.Define<Type, string>(LogLevel.Information, default, "Attach {OutputConsumer} at Docker container {Id}");

    private static readonly Action<ILogger, string, string, Exception> _ConnectToDockerNetwork
      = LoggerMessage.Define<string, string>(LogLevel.Information, default, "Connect Docker container {ContainerId} to Docker network {NetworkId}");

    private static readonly Action<ILogger, string, string, Exception> _ExecuteCommandInDockerContainer
      = LoggerMessage.Define<string, string>(LogLevel.Information, default, "Execute \"{Command}\" at Docker container {Id}");

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
      = LoggerMessage.Define<Guid>(LogLevel.Debug, default, "Cannot get resource reaper {Id} endpoint");

    private static readonly Action<ILogger, Guid, string, Exception> _CanNotConnectToResourceReaper
      = LoggerMessage.Define<Guid, string>(LogLevel.Debug, default, "Cannot connect to resource reaper {Id} at {Endpoint}");

    private static readonly Action<ILogger, Guid, string, Exception> _LostConnectionToResourceReaper
      = LoggerMessage.Define<Guid, string>(LogLevel.Debug, default, "Lost connection to resource reaper {Id} at {Endpoint}");

    private static readonly Action<ILogger, string, Exception> _DockerConfigFileNotFound
      = LoggerMessage.Define<string>(LogLevel.Information, default, "Docker config \"{DockerConfigFilePath}\" not found");

    private static readonly Action<ILogger, string, Exception> _SearchingDockerRegistryCredential
      = LoggerMessage.Define<string>(LogLevel.Information, default, "Searching Docker registry credential in {CredentialStore}");

    private static readonly Action<ILogger, string, JsonValueKind, Exception> _DockerRegistryAuthPropertyValueKindInvalid
      = LoggerMessage.Define<string, JsonValueKind>(LogLevel.Warning, default, "The \"auth\" property value kind for {DockerRegistry} is invalid: {ValueKind}");

    private static readonly Action<ILogger, string, Exception> _DockerRegistryAuthPropertyValueNotFound
      = LoggerMessage.Define<string>(LogLevel.Warning, default, "The \"auth\" property value for {DockerRegistry} not found");

    private static readonly Action<ILogger, string, Exception> _DockerRegistryAuthPropertyValueInvalidBase64
      = LoggerMessage.Define<string>(LogLevel.Warning, default, "The \"auth\" property value for {DockerRegistry} is not a valid Base64 string");

    private static readonly Action<ILogger, string, Exception> _DockerRegistryAuthPropertyValueInvalidBasicAuthenticationFormat
      = LoggerMessage.Define<string>(LogLevel.Warning, default, "The \"auth\" property value for {DockerRegistry} should contain one colon separating the username and the password (basic authentication)");

    private static readonly Action<ILogger, string, Exception> _DockerRegistryCredentialNotFound
      = LoggerMessage.Define<string>(LogLevel.Information, default, "Docker registry credential {DockerRegistry} not found");

    private static readonly Action<ILogger, string, Exception> _DockerRegistryCredentialFound
      = LoggerMessage.Define<string>(LogLevel.Information, default, "Docker registry credential {DockerRegistry} found");

    private static readonly Action<ILogger, Exception> _ReusableExperimentalFeature
      = LoggerMessage.Define(LogLevel.Warning, default, "Reuse is an experimental feature. For more information, visit: https://dotnet.testcontainers.org/api/resource_reuse/");

    private static readonly Action<ILogger, Exception> _ReusableResourceFound
      = LoggerMessage.Define(LogLevel.Information, default, "Reusable resource found");

    private static readonly Action<ILogger, Exception> _ReusableResourceNotFound
      = LoggerMessage.Define(LogLevel.Information, default, "Reusable resource not found, create resource");

#pragma warning restore InconsistentNaming, SA1309

    public static void IgnorePatternAdded(this ILogger logger, Regex ignorePattern)
    {
      _IgnorePatternAdded(logger, ignorePattern, null);
    }

    public static void DockerContainerCreated(this ILogger logger, string id)
    {
      _DockerContainerCreated(logger, TruncId(id), null);
    }

    public static void StartDockerContainer(this ILogger logger, string id)
    {
      _StartDockerContainer(logger, TruncId(id), null);
    }

    public static void StopDockerContainer(this ILogger logger, string id)
    {
      _StopDockerContainer(logger, TruncId(id), null);
    }

    public static void PauseDockerContainer(this ILogger logger, string id)
    {
      _PauseDockerContainer(logger, TruncId(id), null);
    }

    public static void UnpauseDockerContainer(this ILogger logger, string id)
    {
      _UnpauseDockerContainer(logger, TruncId(id), null);
    }

    public static void DeleteDockerContainer(this ILogger logger, string id)
    {
      _DeleteDockerContainer(logger, TruncId(id), null);
    }

    public static void StartReadinessCheck(this ILogger logger, string id)
    {
      _StartReadinessCheck(logger, TruncId(id), null);
    }

    public static void CompleteReadinessCheck(this ILogger logger, string id)
    {
      _CompleteReadinessCheck(logger, TruncId(id), null);
    }

    public static void CopyArchiveToDockerContainer(this ILogger logger, string id, long length)
    {
      _CopyArchiveToDockerContainer(logger, length, TruncId(id), null);
    }

    public static void ReadArchiveFromDockerContainer(this ILogger logger, string id, string path)
    {
      _ReadArchiveFromDockerContainer(logger, path, TruncId(id), null);
    }

    public static void AttachToDockerContainer(this ILogger logger, string id, Type type)
    {
      _AttachToDockerContainer(logger, type, TruncId(id), null);
    }

    public static void ConnectToDockerNetwork(this ILogger logger, string networkId, string containerId)
    {
      _ConnectToDockerNetwork(logger, TruncId(containerId), TruncId(networkId), null);
    }

    public static void ExecuteCommandInDockerContainer(this ILogger logger, string id, IEnumerable<string> command)
    {
      _ExecuteCommandInDockerContainer(logger, string.Join(" ", command), TruncId(id), null);
    }

    public static void DockerImageCreated(this ILogger logger, IImage image)
    {
      _DockerImageCreated(logger, image.FullName, null);
    }

    public static void DockerImageBuilt(this ILogger logger, IImage image)
    {
      _DockerImageBuilt(logger, image.FullName, null);
    }

    public static void DeleteDockerImage(this ILogger logger, IImage image)
    {
      _DeleteDockerImage(logger, image.FullName, null);
    }

    public static void DockerNetworkCreated(this ILogger logger, string id)
    {
      _DockerNetworkCreated(logger, TruncId(id), null);
    }

    public static void DeleteDockerNetwork(this ILogger logger, string id)
    {
      _DeleteDockerNetwork(logger, TruncId(id), null);
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

    public static void DockerRegistryAuthPropertyValueKindInvalid(this ILogger logger, string dockerRegistry, JsonValueKind valueKind)
    {
      _DockerRegistryAuthPropertyValueKindInvalid(logger, dockerRegistry, valueKind, null);
    }

    public static void DockerRegistryAuthPropertyValueNotFound(this ILogger logger, string dockerRegistry)
    {
      _DockerRegistryAuthPropertyValueNotFound(logger, dockerRegistry, null);
    }

    public static void DockerRegistryAuthPropertyValueInvalidBase64(this ILogger logger, string dockerRegistry, Exception e)
    {
      _DockerRegistryAuthPropertyValueInvalidBase64(logger, dockerRegistry, e);
    }

    public static void DockerRegistryAuthPropertyValueInvalidBasicAuthenticationFormat(this ILogger logger, string dockerRegistry)
    {
      _DockerRegistryAuthPropertyValueInvalidBasicAuthenticationFormat(logger, dockerRegistry, null);
    }

    public static void DockerRegistryCredentialNotFound(this ILogger logger, string dockerRegistry)
    {
      _DockerRegistryCredentialNotFound(logger, dockerRegistry, null);
    }

    public static void DockerRegistryCredentialFound(this ILogger logger, string dockerRegistry)
    {
      _DockerRegistryCredentialFound(logger, dockerRegistry, null);
    }

    public static void ReusableExperimentalFeature(this ILogger logger)
    {
      _ReusableExperimentalFeature(logger, null);
    }

    public static void ReusableResourceFound(this ILogger logger)
    {
      _ReusableResourceFound(logger, null);
    }

    public static void ReusableResourceNotFound(this ILogger logger)
    {
      _ReusableResourceNotFound(logger, null);
    }

    private static string TruncId(string id)
    {
      return id.Substring(0, Math.Min(12, id.Length));
    }
  }
}
