namespace DotNet.Testcontainers.Images
{
  using System;
  using Docker.DotNet.Models;

  public static class PullPolicy
  {
    /// <summary>
    /// Gets the policy of never pulling the image.
    /// </summary>
    public static Func<ImagesListResponse, bool> Never
    {
      get
      {
        return _ => false;
      }
    }

    /// <summary>
    /// Gets the policy of pulling the image if it's not cached.
    /// </summary>
    /// <remarks>
    /// This is the default behavior.
    /// </remarks>
    public static Func<ImagesListResponse, bool> Missing
    {
      get
      {
        return cachedImage => cachedImage != null;
      }
    }

    /// <summary>
    /// Gets the policy of always pulling the image.
    /// </summary>
    public static Func<ImagesListResponse, bool> Always
    {
      get
      {
        return _ => true;
      }
    }
  }
}
