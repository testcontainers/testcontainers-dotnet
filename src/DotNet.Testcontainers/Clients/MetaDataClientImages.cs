namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;

  internal sealed class MetaDataClientImages : DockerMetaDataClient<ImagesListResponse>
  {
    private static readonly Lazy<DockerMetaDataClient<ImagesListResponse>> MetaDataClient = new Lazy<DockerMetaDataClient<ImagesListResponse>>(() =>
    {
      return new MetaDataClientImages();
    });

    private MetaDataClientImages()
    {
    }

    internal static DockerMetaDataClient<ImagesListResponse> Instance
    {
      get
      {
        return MetaDataClient.Value;
      }
    }

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

      var reponse = Docker.Images.ListImagesAsync(new ImagesListParameters
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

      return (await reponse).FirstOrDefault();
    }
  }
}
