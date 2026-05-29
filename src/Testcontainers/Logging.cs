namespace DotNet.Testcontainers
{
  using System;
  using System.Collections.Generic;
  using System.Text.Json;
  using System.Text.RegularExpressions;
  using DotNet.Testcontainers.Images;
  using Microsoft.Extensions.Logging;

  internal static partial class Logging
  {
    [LoggerMessage(Level = LogLevel.Information, Message = "Pattern {IgnorePattern} added to the regex cache")]
    private static partial void IgnorePatternAddedCore(ILogger logger, Regex ignorePattern);

    [LoggerMessage(Level = LogLevel.Information, Message = "Docker container {Id} created")]
    private static partial void DockerContainerCreatedCore(ILogger logger, string id);

    [LoggerMessage(Level = LogLevel.Information, Message = "Start Docker container {Id}")]
    private static partial void StartDockerContainerCore(ILogger logger, string id);

    [LoggerMessage(Level = LogLevel.Information, Message = "Stop Docker container {Id}")]
    private static partial void StopDockerContainerCore(ILogger logger, string id);

    [LoggerMessage(Level = LogLevel.Information, Message = "Pause Docker container {Id}")]
    private static partial void PauseDockerContainerCore(ILogger logger, string id);

    [LoggerMessage(Level = LogLevel.Information, Message = "Unpause Docker container {Id}")]
    private static partial void UnpauseDockerContainerCore(ILogger logger, string id);

    [LoggerMessage(Level = LogLevel.Information, Message = "Delete Docker container {Id}")]
    private static partial void DeleteDockerContainerCore(ILogger logger, string id);

    [LoggerMessage(Level = LogLevel.Information, Message = "Wait for Docker container {Id} to complete readiness checks")]
    private static partial void StartReadinessCheckCore(ILogger logger, string id);

    [LoggerMessage(Level = LogLevel.Information, Message = "Docker container {Id} ready")]
    private static partial void CompleteReadinessCheckCore(ILogger logger, string id);

    [LoggerMessage(Level = LogLevel.Information, Message = "Copy tar archive to container: Content length: {Length} byte(s), Docker container: {Id}")]
    private static partial void CopyArchiveToDockerContainerCore(ILogger logger, long length, string id);

    [LoggerMessage(Level = LogLevel.Information, Message = "Read \"{Path}\" from Docker container {Id}")]
    private static partial void ReadArchiveFromDockerContainerCore(ILogger logger, string path, string id);

    [LoggerMessage(Level = LogLevel.Information, Message = "Attach {OutputConsumer} at Docker container {Id}")]
    private static partial void AttachToDockerContainerCore(ILogger logger, Type outputConsumer, string id);

    [LoggerMessage(Level = LogLevel.Information, Message = "Connect Docker container {ContainerId} to Docker network {NetworkId}")]
    private static partial void ConnectToDockerNetworkCore(ILogger logger, string containerId, string networkId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Execute \"{Command}\" at Docker container {Id}")]
    private static partial void ExecuteCommandInDockerContainerCore(ILogger logger, string command, string id);

    [LoggerMessage(Level = LogLevel.Information, Message = "Docker image {FullName} created")]
    private static partial void DockerImageCreatedCore(ILogger logger, string fullName);

    [LoggerMessage(Level = LogLevel.Information, Message = "Docker image {FullName} built")]
    private static partial void DockerImageBuiltCore(ILogger logger, string fullName);

    [LoggerMessage(Level = LogLevel.Information, Message = "Delete Docker image {FullName}")]
    private static partial void DeleteDockerImageCore(ILogger logger, string fullName);

    [LoggerMessage(Level = LogLevel.Information, Message = "Docker network {Id} created")]
    private static partial void DockerNetworkCreatedCore(ILogger logger, string id);

    [LoggerMessage(Level = LogLevel.Information, Message = "Delete Docker network {Id}")]
    private static partial void DeleteDockerNetworkCore(ILogger logger, string id);

    [LoggerMessage(Level = LogLevel.Information, Message = "Docker volume {Name} created")]
    private static partial void DockerVolumeCreatedCore(ILogger logger, string name);

    [LoggerMessage(Level = LogLevel.Information, Message = "Delete Docker volume {Name}")]
    private static partial void DeleteDockerVolumeCore(ILogger logger, string name);

    [LoggerMessage(Level = LogLevel.Information, Message = "{RuntimeInfo}")]
    private static partial void DockerRuntimeInfoCore(ILogger logger, string runtimeInfo);

    [LoggerMessage(Level = LogLevel.Information, Message = "Add file to tar archive: Content length: {Length} byte(s), Target file: \"{Target}\", UID: {Uid}, GID: {Gid}, Mode: {Mode}")]
    private static partial void AddFileToTarArchiveCore(ILogger logger, long length, string target, int uid, int gid, string mode);

    [LoggerMessage(Level = LogLevel.Information, Message = "Add file to tar archive: Source file: \"{Source}\", Target file: \"{Target}\", UID: {Uid}, GID: {Gid}, Mode: {Mode}")]
    private static partial void AddFileToTarArchiveCore(ILogger logger, string source, string target, int uid, int gid, string mode);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Cannot get resource reaper {Id} endpoint")]
    private static partial void CanNotGetResourceReaperEndpointCore(ILogger logger, Guid id, Exception exception);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Cannot connect to resource reaper {Id} at {Endpoint}")]
    private static partial void CanNotConnectToResourceReaperCore(ILogger logger, Guid id, string endpoint, Exception exception);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Lost connection to resource reaper {Id} at {Endpoint}")]
    private static partial void LostConnectionToResourceReaperCore(ILogger logger, Guid id, string endpoint, Exception exception);

    [LoggerMessage(Level = LogLevel.Information, Message = "Docker config \"{DockerConfigFilePath}\" not found")]
    private static partial void DockerConfigFileNotFoundCore(ILogger logger, string dockerConfigFilePath);

    [LoggerMessage(Level = LogLevel.Information, Message = "Searching Docker registry credential in {CredentialStore}")]
    private static partial void SearchingDockerRegistryCredentialCore(ILogger logger, string credentialStore);

    [LoggerMessage(Level = LogLevel.Warning, Message = "The \"auth\" property value kind for {DockerRegistry} is invalid: {ValueKind}")]
    private static partial void DockerRegistryAuthPropertyValueKindInvalidCore(ILogger logger, string dockerRegistry, JsonValueKind valueKind);

    [LoggerMessage(Level = LogLevel.Warning, Message = "The \"auth\" property value for {DockerRegistry} not found")]
    private static partial void DockerRegistryAuthPropertyValueNotFoundCore(ILogger logger, string dockerRegistry);

    [LoggerMessage(Level = LogLevel.Warning, Message = "The \"auth\" property value for {DockerRegistry} is not a valid Base64 string")]
    private static partial void DockerRegistryAuthPropertyValueInvalidBase64Core(ILogger logger, string dockerRegistry, Exception exception);

    [LoggerMessage(Level = LogLevel.Warning, Message = "The \"auth\" property value for {DockerRegistry} should contain one colon separating the username and the password (basic authentication)")]
    private static partial void DockerRegistryAuthPropertyValueInvalidBasicAuthenticationFormatCore(ILogger logger, string dockerRegistry);

    [LoggerMessage(Level = LogLevel.Information, Message = "Docker registry credential {DockerRegistry} not found")]
    private static partial void DockerRegistryCredentialNotFoundCore(ILogger logger, string dockerRegistry);

    [LoggerMessage(Level = LogLevel.Information, Message = "Docker registry credential {DockerRegistry} found")]
    private static partial void DockerRegistryCredentialFoundCore(ILogger logger, string dockerRegistry);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Reuse is an experimental feature. For more information, visit: https://dotnet.testcontainers.org/api/resource_reuse/")]
    private static partial void ReusableExperimentalFeatureCore(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Reusable resource found")]
    private static partial void ReusableResourceFoundCore(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Reusable resource not found, create resource")]
    private static partial void ReusableResourceNotFoundCore(ILogger logger);

    public static void IgnorePatternAdded(this ILogger logger, Regex ignorePattern)
    {
      IgnorePatternAddedCore(logger, ignorePattern);
    }

    public static void DockerContainerCreated(this ILogger logger, string id)
    {
      DockerContainerCreatedCore(logger, TruncId(id));
    }

    public static void StartDockerContainer(this ILogger logger, string id)
    {
      StartDockerContainerCore(logger, TruncId(id));
    }

    public static void StopDockerContainer(this ILogger logger, string id)
    {
      StopDockerContainerCore(logger, TruncId(id));
    }

    public static void PauseDockerContainer(this ILogger logger, string id)
    {
      PauseDockerContainerCore(logger, TruncId(id));
    }

    public static void UnpauseDockerContainer(this ILogger logger, string id)
    {
      UnpauseDockerContainerCore(logger, TruncId(id));
    }

    public static void DeleteDockerContainer(this ILogger logger, string id)
    {
      DeleteDockerContainerCore(logger, TruncId(id));
    }

    public static void StartReadinessCheck(this ILogger logger, string id)
    {
      StartReadinessCheckCore(logger, TruncId(id));
    }

    public static void CompleteReadinessCheck(this ILogger logger, string id)
    {
      CompleteReadinessCheckCore(logger, TruncId(id));
    }

    public static void CopyArchiveToDockerContainer(this ILogger logger, string id, long length)
    {
      CopyArchiveToDockerContainerCore(logger, length, TruncId(id));
    }

    public static void ReadArchiveFromDockerContainer(this ILogger logger, string id, string path)
    {
      ReadArchiveFromDockerContainerCore(logger, path, TruncId(id));
    }

    public static void AttachToDockerContainer(this ILogger logger, string id, Type type)
    {
      AttachToDockerContainerCore(logger, type, TruncId(id));
    }

    public static void ConnectToDockerNetwork(this ILogger logger, string networkId, string containerId)
    {
      ConnectToDockerNetworkCore(logger, TruncId(containerId), TruncId(networkId));
    }

    public static void ExecuteCommandInDockerContainer(this ILogger logger, string id, IEnumerable<string> command)
    {
      ExecuteCommandInDockerContainerCore(logger, string.Join(" ", command), TruncId(id));
    }

    public static void DockerImageCreated(this ILogger logger, IImage image)
    {
      DockerImageCreatedCore(logger, image.FullName);
    }

    public static void DockerImageBuilt(this ILogger logger, IImage image)
    {
      DockerImageBuiltCore(logger, image.FullName);
    }

    public static void DeleteDockerImage(this ILogger logger, IImage image)
    {
      DeleteDockerImageCore(logger, image.FullName);
    }

    public static void DockerNetworkCreated(this ILogger logger, string id)
    {
      DockerNetworkCreatedCore(logger, TruncId(id));
    }

    public static void DeleteDockerNetwork(this ILogger logger, string id)
    {
      DeleteDockerNetworkCore(logger, TruncId(id));
    }

    public static void DockerVolumeCreated(this ILogger logger, string name)
    {
      DockerVolumeCreatedCore(logger, name);
    }

    public static void DeleteDockerVolume(this ILogger logger, string name)
    {
      DeleteDockerVolumeCore(logger, name);
    }

    public static void DockerRuntimeInfo(this ILogger logger, string runtimeInfo)
    {
      DockerRuntimeInfoCore(logger, runtimeInfo);
    }

    public static void AddFileToTarArchive(this ILogger logger, long length, string target, int uid, int gid, string mode)
    {
      AddFileToTarArchiveCore(logger, length, target, uid, gid, mode);
    }

    public static void AddFileToTarArchive(this ILogger logger, string source, string target, int uid, int gid, string mode)
    {
      AddFileToTarArchiveCore(logger, source, target, uid, gid, mode);
    }

    public static void CanNotGetResourceReaperEndpoint(this ILogger logger, Guid id, Exception e)
    {
      CanNotGetResourceReaperEndpointCore(logger, id, logger.IsEnabled(LogLevel.Debug) ? e : null);
    }

    public static void CanNotConnectToResourceReaper(this ILogger logger, Guid id, string host, ushort port, Exception e)
    {
      var endpoint = $"{host}:{port}";
      CanNotConnectToResourceReaperCore(logger, id, endpoint, e);
    }

    public static void LostConnectionToResourceReaper(this ILogger logger, Guid id, string host, ushort port, Exception e)
    {
      var endpoint = $"{host}:{port}";
      LostConnectionToResourceReaperCore(logger, id, endpoint, e);
    }

    public static void DockerConfigFileNotFound(this ILogger logger, string dockerConfigFilePath)
    {
      DockerConfigFileNotFoundCore(logger, dockerConfigFilePath);
    }

    public static void SearchingDockerRegistryCredential(this ILogger logger, string credentialStore)
    {
      SearchingDockerRegistryCredentialCore(logger, credentialStore);
    }

    public static void DockerRegistryAuthPropertyValueKindInvalid(this ILogger logger, string dockerRegistry, JsonValueKind valueKind)
    {
      DockerRegistryAuthPropertyValueKindInvalidCore(logger, dockerRegistry, valueKind);
    }

    public static void DockerRegistryAuthPropertyValueNotFound(this ILogger logger, string dockerRegistry)
    {
      DockerRegistryAuthPropertyValueNotFoundCore(logger, dockerRegistry);
    }

    public static void DockerRegistryAuthPropertyValueInvalidBase64(this ILogger logger, string dockerRegistry, Exception e)
    {
      DockerRegistryAuthPropertyValueInvalidBase64Core(logger, dockerRegistry, e);
    }

    public static void DockerRegistryAuthPropertyValueInvalidBasicAuthenticationFormat(this ILogger logger, string dockerRegistry)
    {
      DockerRegistryAuthPropertyValueInvalidBasicAuthenticationFormatCore(logger, dockerRegistry);
    }

    public static void DockerRegistryCredentialNotFound(this ILogger logger, string dockerRegistry)
    {
      DockerRegistryCredentialNotFoundCore(logger, dockerRegistry);
    }

    public static void DockerRegistryCredentialFound(this ILogger logger, string dockerRegistry)
    {
      DockerRegistryCredentialFoundCore(logger, dockerRegistry);
    }

    public static void ReusableExperimentalFeature(this ILogger logger)
    {
      ReusableExperimentalFeatureCore(logger);
    }

    public static void ReusableResourceFound(this ILogger logger)
    {
      ReusableResourceFoundCore(logger);
    }

    public static void ReusableResourceNotFound(this ILogger logger)
    {
      ReusableResourceNotFoundCore(logger);
    }

    private static string TruncId(string id)
    {
      return id.Substring(0, Math.Min(12, id.Length));
    }
  }
}
