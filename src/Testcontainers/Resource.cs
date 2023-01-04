namespace DotNet.Testcontainers
{
  using System;
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
    protected virtual bool Exists()
    {
      return true;
    }

    /// <summary>
    /// Throws an <see cref="InvalidOperationException" /> when the resources was not found.
    /// </summary>
    /// <exception cref="InvalidOperationException">The resource was not found.</exception>
    protected virtual void ThrowIfResourceNotFound()
    {
      if (this.Exists())
      {
        return;
      }

      var resourceType = this.GetType().Name;
      throw new InvalidOperationException($"Could not find resource '{resourceType}'. Please create the resource by calling StartAsync(CancellationToken) or CreateAsync(CancellationToken) first.");
    }
  }
}
