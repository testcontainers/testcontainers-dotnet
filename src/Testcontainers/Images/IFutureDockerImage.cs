namespace DotNet.Testcontainers.Images
{
  using JetBrains.Annotations;

  /// <summary>
  /// A future image instance.
  /// </summary>
  [PublicAPI]
  public interface IFutureDockerImage : IImage, IFutureResource
  {
  }
}
