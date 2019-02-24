namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
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

    internal override IReadOnlyCollection<ImagesListResponse> GetAll()
    {
      return Docker.Images.ListImagesAsync(new ImagesListParameters { }).Result.ToList();
    }

    internal override ImagesListResponse ById(string id)
    {
      return string.IsNullOrWhiteSpace(id) ? null : this.GetAll().FirstOrDefault(value => id.Equals(value.ID));
    }

    internal override ImagesListResponse ByName(string name)
    {
      return string.IsNullOrWhiteSpace(name) ? null : this.ByProperty("label", name);
    }

    internal override ImagesListResponse ByProperty(string property, string value)
    {
      return Docker.Images.ListImagesAsync(new ImagesListParameters
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
      }).Result.FirstOrDefault();
    }
  }
}
