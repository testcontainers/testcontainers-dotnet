namespace DotNet.Testcontainers
{
  using System;
  using System.Diagnostics;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;

  /// <summary>
  /// Nullability preconditions.
  /// </summary>
  internal static partial class Guard
  {
    /// <summary>
    /// Ensures that an argument value is null.
    /// </summary>
    /// <param name="argument">Argument to validate.</param>
    /// <typeparam name="TType">Type of the argument.</typeparam>
    /// <returns>Reference to the Guard object that validates the argument preconditions.</returns>
    /// <exception cref="ArgumentException">Thrown when argument is not null.</exception>
    [PublicAPI]
    [DebuggerStepThrough]
    public static ref readonly ArgumentInfo<TType> Null<TType>(in this ArgumentInfo<TType> argument)
      where TType : class
    {
      if (argument.HasValue())
      {
        throw new ArgumentException($"{argument.Name} must be null.", argument.Name);
      }

      return ref argument;
    }

    /// <summary>
    /// Ensures that an argument value is not null.
    /// </summary>
    /// <param name="argument">Argument to validate.</param>
    /// <typeparam name="TType">Type of the argument.</typeparam>
    /// <returns>Reference to the Guard object that validates the argument preconditions.</returns>
    /// <exception cref="ArgumentNullException">Thrown when argument is null.</exception>
    [PublicAPI]
    [DebuggerStepThrough]
    public static ref readonly ArgumentInfo<TType> NotNull<TType>(in this ArgumentInfo<TType> argument)
      where TType : class
    {
      if (!argument.HasValue())
      {
        throw new ArgumentNullException(argument.Name, $"{argument.Name} can not be null.");
      }

      return ref argument;
    }

    /// <summary>
    /// Ensures that the Docker endpoint authentication configuration is set.
    /// </summary>
    /// <param name="argument">The Docker endpoint authentication configuration.</param>
    /// <typeparam name="TType">An implementation of <see cref="IDockerEndpointAuthenticationConfiguration" />.</typeparam>
    /// <returns>Reference to the Guard object that validates the argument preconditions.</returns>
    /// <exception cref="ArgumentNullException">Thrown when argument is null.</exception>
    [PublicAPI]
    [DebuggerStepThrough]
    public static ref readonly ArgumentInfo<TType> DockerEndpointAuthConfigIsSet<TType>(in this ArgumentInfo<TType> argument)
      where TType : IDockerEndpointAuthenticationConfiguration
    {
      if (argument.HasValue())
      {
        return ref argument;
      }

      const string message = "Cannot detect the Docker endpoint. Use either the environment variables or the ~/.testcontainers.properties file to customize your configuration:\nhttps://www.testcontainers.org/features/configuration/#customizing-docker-host-detection";
      throw new ArgumentNullException(argument.Name, message);
    }
  }
}
