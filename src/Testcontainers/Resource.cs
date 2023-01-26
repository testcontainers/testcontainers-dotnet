namespace DotNet.Testcontainers
{
  using System;
  using System.Globalization;
  using JetBrains.Annotations;

  /// <summary>
  /// A resource instance.
  /// </summary>
  [PublicAPI]
  public abstract class Resource
  {
    /// <summary>
    /// Checks whether the resources exists or not.
    /// </summary>
    /// <returns>True if the resource exists; otherwise, false.</returns>
    protected abstract bool Exists();

    /// <summary>
    /// Throws an <see cref="InvalidOperationException" /> when the resources was not found.
    /// </summary>
    /// <exception cref="InvalidOperationException">The resource was not found.</exception>
    protected virtual void ThrowIfResourceNotFound()
    {
      const string message = "Could not find resource '{0}'. Please create the resource by calling StartAsync(CancellationToken) or CreateAsync(CancellationToken).";
      _ = Guard.Argument(this, this.GetType().Name)
        .ThrowIf(argument => !argument.Value.Exists(), argument => new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, message, argument.Name)));
    }
  }
}
