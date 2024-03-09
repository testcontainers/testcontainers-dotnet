namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using System.Security.Cryptography;
  using System.Text.Json;
  using System.Text.Json.Serialization;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;

  /// <inheritdoc cref="IResourceConfiguration{TCreateResourceEntity}" />
  [PublicAPI]
  public class ResourceConfiguration<TCreateResourceEntity> : IResourceConfiguration<TCreateResourceEntity>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceConfiguration{TCreateResourceEntity}" /> class.
    /// </summary>
    /// <param name="dockerEndpointAuthenticationConfiguration">The Docker endpoint authentication configuration.</param>
    /// <param name="labels">The test session id.</param>
    /// <param name="parameterModifiers">A list of low level modifications of the Docker.DotNet entity.</param>
    /// <param name="reuse">A value indicating whether to reuse an existing resource configuration or not.</param>
    public ResourceConfiguration(
      IDockerEndpointAuthenticationConfiguration dockerEndpointAuthenticationConfiguration = null,
      IReadOnlyDictionary<string, string> labels = null,
      IReadOnlyList<Action<TCreateResourceEntity>> parameterModifiers = null,
      bool? reuse = null)
    {
      SessionId = labels != null && labels.TryGetValue(ResourceReaper.ResourceReaperSessionLabel, out var resourceReaperSessionId) && Guid.TryParseExact(resourceReaperSessionId, "D", out var sessionId) ? sessionId : Guid.Empty;
      DockerEndpointAuthConfig = dockerEndpointAuthenticationConfiguration;
      Labels = labels;
      ParameterModifiers = parameterModifiers;
      Reuse = reuse;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceConfiguration{TCreateResourceEntity}" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    protected ResourceConfiguration(IResourceConfiguration<TCreateResourceEntity> resourceConfiguration)
      : this(new ResourceConfiguration<TCreateResourceEntity>(), resourceConfiguration)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceConfiguration{TCreateResourceEntity}" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    protected ResourceConfiguration(IResourceConfiguration<TCreateResourceEntity> oldValue, IResourceConfiguration<TCreateResourceEntity> newValue)
      : this(
        dockerEndpointAuthenticationConfiguration: BuildConfiguration.Combine(oldValue.DockerEndpointAuthConfig, newValue.DockerEndpointAuthConfig),
        labels: BuildConfiguration.Combine(oldValue.Labels, newValue.Labels),
        parameterModifiers: BuildConfiguration.Combine(oldValue.ParameterModifiers, newValue.ParameterModifiers),
        reuse: (oldValue.Reuse.HasValue && oldValue.Reuse.Value) || (newValue.Reuse.HasValue && newValue.Reuse.Value))
    {
    }

    /// <inheritdoc />
    [JsonIgnore]
    public Guid SessionId { get; }

    /// <inheritdoc />
    [JsonIgnore]
    public bool? Reuse { get; }

    /// <inheritdoc />
    [JsonIgnore]
    public IDockerEndpointAuthenticationConfiguration DockerEndpointAuthConfig { get; }

    /// <inheritdoc />
    [JsonConverter(typeof(JsonIgnoreRuntimeResourceLabels))]
    public IReadOnlyDictionary<string, string> Labels { get; }

    /// <inheritdoc />
    [JsonIgnore]
    public IReadOnlyList<Action<TCreateResourceEntity>> ParameterModifiers { get; }

    /// <inheritdoc />
    public virtual string GetReuseHash()
    {
      var jsonUtf8Bytes = JsonSerializer.SerializeToUtf8Bytes(this, GetType());

#if NET6_0_OR_GREATER
      return Convert.ToBase64String(SHA1.HashData(jsonUtf8Bytes));
#else
      using (var sha1 = SHA1.Create())
      {
        return Convert.ToBase64String(sha1.ComputeHash(jsonUtf8Bytes));
      }
#endif
    }
  }
}
