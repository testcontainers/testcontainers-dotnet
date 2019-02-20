namespace DotNet.Testcontainers.Clients.MetaData
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

    public static DockerMetaDataClient<ImagesListResponse> Instance
    {
      get
      {
        return MetaDataClient.Value;
      }
    }

    public override ICollection<ImagesListResponse> All
    {
      get
      {
        return Docker.Images.ListImagesAsync(new ImagesListParameters { }).Result.ToList();
      }
    }

    public override ImagesListResponse ById(string id)
    {
      return this.ByProperty("id", id);
    }

    public override ImagesListResponse ByName(string name)
    {
      return this.ByProperty("label", name);
    }

    protected override ImagesListResponse ByProperty(string property, string value)
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
