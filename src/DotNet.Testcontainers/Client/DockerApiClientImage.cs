namespace DotNet.Testcontainers.Client
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;

  /// <summary>
  /// This class represents a Docker API client and interacts with Docker images.
  /// </summary>
  internal sealed class DockerApiClientImage : AbstractContainerImageClient<ImagesListResponse>
  {
    private static readonly Lazy<DockerApiClientImage> MetaDataClientLazy = new Lazy<DockerApiClientImage>(() => new DockerApiClientImage());

    private DockerApiClientImage()
    {
    }

    internal static DockerApiClientImage Instance { get; } = MetaDataClientLazy.Value;

    internal override async Task<IReadOnlyCollection<ImagesListResponse>> GetAllAsync()
    {
      return (await Docker.Images.ListImagesAsync(new ImagesListParameters { All = true })).ToList();
    }

    internal override async Task<ImagesListResponse> ByIdAsync(string id)
    {
      return (await this.GetAllAsync()).FirstOrDefault(image => image.ID.Equals(id));
    }

    internal override async Task<ImagesListResponse> ByNameAsync(string name)
    {
      return await this.ByPropertyAsync("label", name);
    }

    internal override async Task<ImagesListResponse> ByPropertyAsync(string property, string value)
    {
      if (string.IsNullOrWhiteSpace(value))
      {
        return null;
      }

      var response = Docker.Images.ListImagesAsync(new ImagesListParameters
      {
        All = true,
        Filters = new Dictionary<string, IDictionary<string, bool>>
        {
          {
            property, new Dictionary<string, bool>
            {
              { value, true },
            }
          },
        },
      });

      return (await response).FirstOrDefault();
    }
  }
}
